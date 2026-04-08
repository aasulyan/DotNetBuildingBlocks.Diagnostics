# DotNetBuildingBlocks.Observability

OpenTelemetry-based observability setup, instrumentation registration, and exporter configuration for the DotNetBuildingBlocks Diagnostics solution.

## Purpose

This package is the **host-facing OpenTelemetry integration layer** for DotNetBuildingBlocks.
It configures the OpenTelemetry SDK, resource metadata, instrumentations, and exporters in a single, options-driven registration call.

## What this package provides

- OpenTelemetry SDK registration (tracing + metrics pipelines)
- Resource metadata configuration (service name, version, namespace, environment, instance id, custom attributes)
- Instrumentation registration (ASP.NET Core, HttpClient, Runtime, Process)
- Exporter registration (OTLP)
- Coordinated enable/disable switches for tracing, metrics, and logging pipelines
- Composition with the generic `DotNetBuildingBlocks.Observation` layer

## What this package does NOT provide

- Business-specific metrics or activities
- Application-specific instrumentation rules
- Service-specific log/event catalogs
- Hardcoded environment-specific endpoints
- Serilog-specific integration (use `DotNetBuildingBlocks.Serilog`)
- Generic tracing/metrics helpers (use `DotNetBuildingBlocks.Tracing` / `DotNetBuildingBlocks.Metrics`)

## When to use this package

Use **Observability** when your host application needs:

- OpenTelemetry SDK wiring
- Trace and metric export to a collector (OTLP)
- ASP.NET Core, HttpClient, Runtime, or Process instrumentation
- Resource metadata for distributed tracing backends

## When to use Observation instead

Use **Observation** when you only need:

- Generic diagnostics composition (Logging + Tracing + Metrics)
- Shared service identity and naming conventions
- No OpenTelemetry SDK, exporters, or vendor-specific setup

## Installation

```shell
dotnet add package DotNetBuildingBlocks.Observability
```

## Usage

```csharp
services.AddDotNetBuildingBlocksObservability(options =>
{
    options.ServiceName = "Orders.Service";
    options.ServiceVersion = "1.0.0";

    options.EnableTracing = true;
    options.EnableMetrics = true;
    options.EnableLogging = false;

    options.Resource.ServiceNamespace = "orders";
    options.Resource.DeploymentEnvironment = "production";

    options.Exporters.Otlp.Enabled = true;
    options.Exporters.Otlp.Endpoint = "http://localhost:4317";

    options.Instrumentations.AspNetCore.Enabled = true;
    options.Instrumentations.HttpClient.Enabled = true;
    options.Instrumentations.Runtime.Enabled = true;
    options.Instrumentations.Process.Enabled = true;
});
```

## Package boundaries

| Package | Responsibility |
|---------|---------------|
| `DotNetBuildingBlocks.Logging` | Generic structured logging conventions and helpers |
| `DotNetBuildingBlocks.Tracing` | Generic ActivitySource, activity helpers, trace context |
| `DotNetBuildingBlocks.Metrics` | Generic Meter, instruments, metric tag helpers |
| `DotNetBuildingBlocks.Observation` | Thin composition over Logging + Tracing + Metrics |
| **`DotNetBuildingBlocks.Observability`** | **OpenTelemetry SDK setup, instrumentations, exporters** |
| `DotNetBuildingBlocks.Serilog` | Serilog-specific integration |

## Dependencies

- `DotNetBuildingBlocks.Observation` (which transitively includes Logging, Tracing, Metrics)
- `OpenTelemetry`
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Exporter.OpenTelemetryProtocol`
- `OpenTelemetry.Instrumentation.AspNetCore`
- `OpenTelemetry.Instrumentation.Http`
- `OpenTelemetry.Instrumentation.Runtime`
- `OpenTelemetry.Instrumentation.Process`

## Supported instrumentations

| Instrumentation | Pipeline | Toggle |
|----------------|----------|--------|
| ASP.NET Core | Tracing + Metrics | `Instrumentations.AspNetCore.Enabled` |
| HttpClient | Tracing + Metrics | `Instrumentations.HttpClient.Enabled` |
| .NET Runtime | Metrics | `Instrumentations.Runtime.Enabled` |
| Process | Metrics | `Instrumentations.Process.Enabled` |

## Supported exporters

| Exporter | Toggle | Configuration |
|----------|--------|---------------|
| OTLP | `Exporters.Otlp.Enabled` | `Endpoint`, `Protocol`, `Headers` |

## Future extension points

- Additional exporters (Console, Prometheus, Zipkin)
- Additional instrumentations (gRPC, Entity Framework Core)
- `IConfiguration` binding for `ObservabilityOptions`
- `IHostApplicationBuilder` overload
- Selective pipeline builders for advanced scenarios
- Log pipeline OpenTelemetry export support
