using DotNetBuildingBlocks.Metrics.Abstractions;
using DotNetBuildingBlocks.Metrics.Tags;
using DotNetBuildingBlocks.Observability.DependencyInjection;
using DotNetBuildingBlocks.Tracing.Abstractions;
using DotNetBuildingBlocks.Tracing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ──────────────────────────────────────────────────────────────
// DotNetBuildingBlocks.Observability Sample
//
// Demonstrates the full OpenTelemetry observability pipeline:
//   - Resource metadata configuration
//   - Tracing + Metrics pipelines with OTLP export
//   - ASP.NET Core / HttpClient / Runtime / Process instrumentations
//   - Using generic Tracing + Metrics building blocks inside the pipeline
// ──────────────────────────────────────────────────────────────

var services = new ServiceCollection();

// Register console logging so we can see log output.
services.AddLogging(logging => logging.AddConsole());

// Register the full observability pipeline.
services.AddDotNetBuildingBlocksObservability(options =>
{
    // Service identity (required).
    options.ServiceName = "Samples.OrderProcessor";
    options.ServiceVersion = "1.0.0";

    // Pipeline toggles.
    options.EnableTracing = true;
    options.EnableMetrics = true;
    options.EnableLogging = true;

    // Resource metadata.
    options.Resource.ServiceNamespace = "samples";
    options.Resource.DeploymentEnvironment = "development";
    options.Resource.Attributes["team"] = "platform";

    // Instrumentations (toggle what you need).
    options.Instrumentations.HttpClient.Enabled = true;
    // options.Instrumentations.AspNetCore.Enabled = true;  // enable in ASP.NET Core hosts
    // options.Instrumentations.Runtime.Enabled = true;
    // options.Instrumentations.Process.Enabled = true;

    // OTLP exporter (point to your collector).
    // options.Exporters.Otlp.Enabled = true;
    // options.Exporters.Otlp.Endpoint = "http://localhost:4317";
});

using var provider = services.BuildServiceProvider();

// Resolve the generic building blocks that Observability registered for us.
var logger = provider.GetRequiredService<ILogger<OrderProcessor>>();
var activitySourceAccessor = provider.GetRequiredService<IActivitySourceAccessor>();
var meterAccessor = provider.GetRequiredService<IMeterAccessor>();

var processor = new OrderProcessor(logger, activitySourceAccessor, meterAccessor);

// Simulate processing some orders.
processor.ProcessOrder("ORD-001", "tenant-acme");
processor.ProcessOrder("ORD-002", "tenant-globex");

Console.WriteLine();
Console.WriteLine("Sample completed. In a real setup with OTLP enabled,");
Console.WriteLine("traces and metrics would be exported to your collector.");

// ──────────────────────────────────────────────────────────────
// Sample service that uses all three diagnostics building blocks.
// ──────────────────────────────────────────────────────────────

internal sealed class OrderProcessor
{
    private readonly ILogger<OrderProcessor> _logger;
    private readonly IActivitySourceAccessor _activitySource;
    private readonly IMeterAccessor _meter;

    public OrderProcessor(
        ILogger<OrderProcessor> logger,
        IActivitySourceAccessor activitySource,
        IMeterAccessor meter)
    {
        _logger = logger;
        _activitySource = activitySource;
        _meter = meter;
    }

    public void ProcessOrder(string orderId, string tenantId)
    {
        // Start a traced activity for this operation.
        using var activity = _activitySource.StartInternalActivity("ProcessOrder");
        activity?
            .SetOperationName("ProcessOrder")
            .SetCorrelationId(Guid.NewGuid().ToString("N"))
            .SetTenantId(tenantId)
            .SetEntity("Order", orderId);

        _logger.LogInformation("Processing order {OrderId} for tenant {TenantId}", orderId, tenantId);

        // Simulate work.
        Thread.Sleep(50);

        // Record a metric.
        var counter = _meter.CreateCounter<long>("orders.processed", description: "Total orders processed");
        counter.Add(1,
            MetricTags.Operation("ProcessOrder"),
            MetricTags.TenantId(tenantId),
            MetricTags.Outcome("success"));

        var histogram = _meter.CreateHistogram<double>("orders.duration_ms", "ms", "Order processing duration");
        histogram.Record(50.0,
            MetricTags.Operation("ProcessOrder"),
            MetricTags.TenantId(tenantId));

        activity?.MarkSuccess();

        _logger.LogInformation("Order {OrderId} processed successfully", orderId);
    }
}
