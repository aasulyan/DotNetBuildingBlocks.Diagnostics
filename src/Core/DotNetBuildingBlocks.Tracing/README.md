# DotNetBuildingBlocks.Tracing

Provider-agnostic tracing helpers for .NET applications built on `System.Diagnostics.Activity`.

## What this package provides

- `IActivitySourceAccessor` / `ActivitySourceAccessor` — managed `ActivitySource` wrapper with DI support
- `TracingOptions` — configuration model for service name, version, and `ActivitySource` name
- `AddDotNetBuildingBlocksTracing(...)` — one-call DI registration
- `ActivityExtensions` — convenience extensions for starting and tagging activities
- `TraceContextSnapshot` — lightweight capture of current trace context
- `TracingTagNames` — well-known tag name constants

## What this package does not provide

This package intentionally does **not** configure:
- OpenTelemetry SDK or exporters
- Serilog or any logging provider
- ASP.NET Core middleware
- Vendor-specific integrations

## Installation

```bash
dotnet add package DotNetBuildingBlocks.Tracing
```

## Basic usage

```csharp
using DotNetBuildingBlocks.Tracing.DependencyInjection;

services.AddDotNetBuildingBlocksTracing(options =>
{
    options.ServiceName = "Orders.Service";
    options.ServiceVersion = "1.0.0";
});
```

```csharp
public class OrderProcessor
{
    private readonly IActivitySourceAccessor _activitySource;

    public OrderProcessor(IActivitySourceAccessor activitySource)
    {
        _activitySource = activitySource;
    }

    public void Process(int orderId)
    {
        using var activity = _activitySource.StartInternalActivity("ProcessOrder");
        // ...
    }
}
```
