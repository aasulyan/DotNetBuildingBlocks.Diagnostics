# DotNetBuildingBlocks.Metrics

Provider-agnostic metrics helpers for .NET applications built on `System.Diagnostics.Metrics`.

## What this package provides

- `IMeterAccessor` / `MeterAccessor` — managed `Meter` wrapper with DI support
- `MetricsOptions` — configuration model for service name, version, and `Meter` name
- `AddDotNetBuildingBlocksMetrics(...)` — one-call DI registration
- `CounterExtensions` / `HistogramExtensions` — convenience extensions for instrument creation
- `MetricTagNames` / `MetricTags` — well-known tag name constants and tag helpers

## What this package does not provide

This package intentionally does **not** configure:
- OpenTelemetry SDK or exporters
- Prometheus / OTLP / Graphite exporters
- Serilog or any logging provider
- ASP.NET Core middleware
- Vendor-specific integrations

## Installation

```bash
dotnet add package DotNetBuildingBlocks.Metrics
```

## Basic usage

```csharp
using DotNetBuildingBlocks.Metrics.DependencyInjection;

services.AddDotNetBuildingBlocksMetrics(options =>
{
    options.ServiceName = "Orders.Service";
    options.ServiceVersion = "1.0.0";
});
```

```csharp
public class OrderProcessor
{
    private readonly Counter<long> _ordersProcessed;

    public OrderProcessor(IMeterAccessor meterAccessor)
    {
        _ordersProcessed = meterAccessor.CreateCounter<long>("orders.processed");
    }

    public void Process(int orderId)
    {
        _ordersProcessed.Add(1);
        // ...
    }
}
```
