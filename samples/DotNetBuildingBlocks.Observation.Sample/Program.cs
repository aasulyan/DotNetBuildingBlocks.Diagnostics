using System.Diagnostics;
using System.Diagnostics.Metrics;
using DotNetBuildingBlocks.Metrics.Abstractions;
using DotNetBuildingBlocks.Observation.DependencyInjection;
using DotNetBuildingBlocks.Tracing.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
});

services.AddDotNetBuildingBlocksObservation(options =>
{
    options.ServiceName = "Samples.OrderProcessor";
    options.ServiceVersion = "1.0.0";
    options.ConfigureLogging = true;
    options.ConfigureTracing = true;
    options.ConfigureMetrics = true;
});

services.AddTransient<OrderProcessor>();

using var provider = services.BuildServiceProvider();

var processor = provider.GetRequiredService<OrderProcessor>();
await processor.ProcessAsync(Guid.NewGuid());

internal sealed class OrderProcessor
{
    private readonly ILogger<OrderProcessor> _logger;
    private readonly IActivitySourceAccessor _activitySourceAccessor;
    private readonly Counter<long> _requests;
    private readonly Histogram<double> _duration;

    public OrderProcessor(
        ILogger<OrderProcessor> logger,
        IActivitySourceAccessor activitySourceAccessor,
        IMeterAccessor meterAccessor)
    {
        _logger = logger;
        _activitySourceAccessor = activitySourceAccessor;
        _requests = meterAccessor.CreateCounter<long>("orders.processed", description: "Total number of processed orders.");
        _duration = meterAccessor.CreateHistogram<double>("orders.duration", unit: "ms", description: "Order processing duration.");
    }

    public async Task ProcessAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["OperationName"] = "ProcessOrder",
            ["OrderId"] = orderId
        });

        using var activity = _activitySourceAccessor.StartInternalActivity("ProcessOrder");
        var startedAt = Stopwatch.GetTimestamp();

        _logger.LogInformation("Starting order processing for {OrderId}.", orderId);
        _requests.Add(1, new KeyValuePair<string, object?>("operation", "ProcessOrder"));

        try
        {
            await Task.Delay(150, cancellationToken);
            _duration.Record(
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                new KeyValuePair<string, object?>("operation", "ProcessOrder"),
                new KeyValuePair<string, object?>("outcome", "success"));

            _logger.LogInformation("Order processing completed for {OrderId}.", orderId);
        }
        catch (Exception exception)
        {
            _duration.Record(
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                new KeyValuePair<string, object?>("operation", "ProcessOrder"),
                new KeyValuePair<string, object?>("outcome", "failure"));

            _logger.LogError(exception, "Order processing failed for {OrderId}.", orderId);
            throw;
        }
    }
}
