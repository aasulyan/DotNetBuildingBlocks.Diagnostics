using System.Diagnostics;
using DotNetBuildingBlocks.Logging.Context;
using DotNetBuildingBlocks.Logging.DependencyInjection;
using DotNetBuildingBlocks.Serilog.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices(services =>
{
    services.AddDotNetBuildingBlocksLogging();
    services.AddTransient<OrderProcessor>();
});

hostBuilder.UseDotNetBuildingBlocksSerilog(options =>
{
    options.ApplicationName = "Samples.OrderWorker";
    options.ApplicationVersion = "1.0.0";
    options.UseConsole = true;
    options.IncludeActivityEnricher = true;
    options.IncludeCorrelationEnricher = true;
    options.IncludeMachineName = true;
    options.IncludeEnvironmentName = true;
    options.ReadFromConfiguration = false;
});

using var host = hostBuilder.Build();

var processor = host.Services.GetRequiredService<OrderProcessor>();

await processor.ProcessAsync(orderId: 1001);
await processor.ProcessAsync(orderId: 1002, simulateFailure: true);

Console.WriteLine();
Console.WriteLine("Sample finished. Press Ctrl+C to exit.");

internal sealed class OrderProcessor
{
    private static readonly ActivitySource ActivitySource = new("Samples.OrderWorker");

    private readonly ILogger<OrderProcessor> logger;

    public OrderProcessor(ILogger<OrderProcessor> logger)
    {
        this.logger = logger;
    }

    public async Task ProcessAsync(int orderId, bool simulateFailure = false)
    {
        var correlationId = Guid.NewGuid().ToString("N");

        using var activity = ActivitySource.StartActivity("ProcessOrder", ActivityKind.Internal);
        activity?.AddBaggage("CorrelationId", correlationId);

        using var operationScope = logger.BeginOperationScope("ProcessOrder", correlationId);
        using var entityScope = logger.BeginEntityScope("Order", orderId);

        logger.LogInformation("Order processing started for {OrderId}.", orderId);

        try
        {
            await Task.Delay(50);

            if (simulateFailure)
            {
                throw new InvalidOperationException("Simulated payment failure.");
            }

            logger.LogInformation("Order processing completed for {OrderId}.", orderId);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Order processing failed for {OrderId}.", orderId);
        }
    }
}
