# DotNetBuildingBlocks.Serilog

Serilog-specific integration for the `DotNetBuildingBlocks` Diagnostics solution.

## Purpose

`DotNetBuildingBlocks.Serilog` provides the vendor-specific glue that lets services adopt
[Serilog](https://serilog.net/) consistently across the `DotNetBuildingBlocks` ecosystem
without leaking Serilog references into the generic `DotNetBuildingBlocks.Logging` package.

It exists so that:

- generic structured logging conventions stay in `DotNetBuildingBlocks.Logging`
- Serilog provider setup, enrichers, and `LoggerConfiguration` helpers live here
- consumers can opt into Serilog with a single, predictable host registration call
- the same registration works in console apps, workers, generic hosts, and ASP.NET Core apps

## When to use it

Use this package when your service has standardized on Serilog and you want a small,
opinionated way to:

- bridge `Microsoft.Extensions.Logging` and Serilog
- attach stable application identity properties (`ApplicationName`, `ApplicationVersion`)
  to every log event
- enrich logs with `TraceId` / `SpanId` from `Activity.Current`
- flow `CorrelationId` from `Serilog.Context.LogContext`, Microsoft logger scopes, or
  `Activity.Current` baggage
- read additional Serilog settings from `IConfiguration` (`appsettings.json`)

## What this package provides

- `DotNetSerilogOptions` &mdash; the focused options model
- `IHostBuilder.UseDotNetBuildingBlocksSerilog(...)` &mdash; the primary host registration entry point
- `IServiceCollection.AddDotNetBuildingBlocksSerilog(...)` &mdash; service-collection registration
- `LoggerConfiguration.ApplyDotNetBuildingBlocksDefaults(...)` &mdash; for manual logger composition
- `ApplicationIdentityEnricher` &mdash; adds `ApplicationName` and `ApplicationVersion`
- `ActivityEnricher` &mdash; adds `TraceId`, `SpanId`, and `ParentSpanId` from `Activity.Current`
- optional console / debug sinks via the options model
- optional `IConfiguration` binding via `Serilog.Settings.Configuration`

## What this package intentionally does not provide

- Seq, Elastic, Graylog, Application Insights, or other backend sinks
- file, database, or transport-specific sinks
- HTTP exception handling middleware
- request / response body logging
- ASP.NET Core request logging helpers (use `Serilog.AspNetCore` directly when needed)
- OpenTelemetry SDK setup, exporters, or instrumentations
- metrics or tracing implementations
- business-domain logging extensions
- a giant logging "framework"

This package keeps the public API small on purpose. Anything beyond Serilog provider setup
and a few stable enrichers belongs in a different package.

## Relationship to the rest of the ecosystem

- `DotNetBuildingBlocks.Logging` &mdash; the generic structured logging conventions package.
  It does not depend on Serilog. This package depends on it and aligns property names where
  practical.
- `DotNetBuildingBlocks.Tracing` &mdash; the generic tracing helpers package. This package does
  not depend on it; it reads `Activity.Current` directly so the activity enricher works
  whether or not the tracing package is registered.
- `DotNetBuildingBlocks.Metrics` / `DotNetBuildingBlocks.Observation` /
  `DotNetBuildingBlocks.Observability` &mdash; not referenced. Pair them at the application
  composition root if you need full OpenTelemetry observability.

## Installation

```bash
dotnet add package DotNetBuildingBlocks.Serilog
```

## Host registration

The recommended entry point is `IHostBuilder.UseDotNetBuildingBlocksSerilog`. It works for
worker services, generic hosts, and ASP.NET Core apps because
`WebApplicationBuilder.Host` implements `IHostBuilder`.

### Worker / generic host

```csharp
using DotNetBuildingBlocks.Serilog.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.UseDotNetBuildingBlocksSerilog(options =>
{
    options.ApplicationName = "Samples.OrderWorker";
    options.ApplicationVersion = "1.0.0";
    options.UseConsole = true;
    options.IncludeActivityEnricher = true;
    options.IncludeCorrelationEnricher = true;
});

using var host = builder.Build();
await host.RunAsync();
```

### ASP.NET Core minimal API

```csharp
using DotNetBuildingBlocks.Serilog.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDotNetBuildingBlocksSerilog(options =>
{
    options.ApplicationName = "Samples.OrderApi";
    options.ApplicationVersion = "1.0.0";
    options.UseConsole = true;
});

var app = builder.Build();

app.MapGet("/", () => "Hello, world!");
app.Run();
```

## Manual `LoggerConfiguration` composition

Use `ApplyDotNetBuildingBlocksDefaults` if you need to build a `Logger` outside of the host.

```csharp
using DotNetBuildingBlocks.Serilog.Configuration;
using DotNetBuildingBlocks.Serilog.Options;
using Serilog;

var logger = new LoggerConfiguration()
    .ApplyDotNetBuildingBlocksDefaults(
        new DotNetSerilogOptions
        {
            ApplicationName = "Samples.Manual",
            ApplicationVersion = "1.0.0",
            UseConsole = true
        },
        environmentName: "Development")
    .CreateLogger();

logger.Information("Manual logger ready.");
```

## Enrichers

| Enricher | Properties added | Notes |
|---|---|---|
| `ApplicationIdentityEnricher` | `ApplicationName`, `ApplicationVersion` | Always applied. `ApplicationVersion` is added only when supplied. |
| `ActivityEnricher` | `TraceId`, `SpanId`, `ParentSpanId` | Read from `Activity.Current`. Skipped when no activity exists. |
| Internal correlation enricher | `CorrelationId` | Promotes `CorrelationId` from `Activity.Current` baggage when not already present on the event. |
| Built-in machine name | `MachineName` | Static, set at configuration time. |
| Built-in environment name | `EnvironmentName` | Read from `IHostEnvironment` or `DOTNET_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT`. |
| Built-in thread id | `ThreadId` | Off by default. |

`Enrich.FromLogContext()` is always wired so any properties pushed via
`Serilog.Context.LogContext.PushProperty(...)` &mdash; or via Microsoft logger scopes
exposed by `Serilog.Extensions.Logging` &mdash; flow into structured output.

## Activity / correlation enrichment

When `IncludeActivityEnricher` is enabled, every log event written while an
`Activity.Current` exists includes:

```text
TraceId       = <hex trace id>
SpanId        = <hex span id>
ParentSpanId  = <hex parent span id, if any>
```

```csharp
using var activity = new Activity("ProcessOrder").Start();
logger.LogInformation("Processing order {OrderId}.", 42);
```

The internal correlation enricher additionally promotes a `CorrelationId` value from
`Activity.Current.Baggage` when the log event does not already carry one. Pushing the
correlation id directly via `LogContext` (or via a Microsoft logger scope) is preferred
and always wins.

## Configuration

When `ReadFromConfiguration` is enabled (the default), the package reads the
configured Serilog section via `Serilog.Settings.Configuration`. The package then applies
its own defaults on top, so explicit code-based options take precedence over configuration.

```jsonc
// appsettings.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" }
    ]
  }
}
```

## Configuration precedence

1. `Serilog.Settings.Configuration` reads the configured `Serilog` section (if `ReadFromConfiguration` is enabled).
2. The package then calls `ApplyDotNetBuildingBlocksDefaults`, which sets the minimum level,
   adds the application identity / activity / correlation enrichers, and enables the
   console / debug sinks based on the options.

In other words, code-based options applied through `DotNetSerilogOptions` always win over
values read from configuration. This is deterministic and the same in every host type.

## Sample output

A typical log event written by a worker looks like this when the JSON formatter is enabled:

```json
{
  "@t": "2026-04-10T08:30:00.1234567Z",
  "@mt": "Processing order {OrderId}.",
  "@l": "Information",
  "OrderId": 42,
  "ApplicationName": "Samples.OrderWorker",
  "ApplicationVersion": "1.0.0",
  "EnvironmentName": "Development",
  "MachineName": "LOCAL-DEV-01",
  "TraceId": "8a3c60f7d188f8fa79d48a392c986a36",
  "SpanId": "1f1d6a2e1f6f3b8c",
  "CorrelationId": "0d3f8c4a-a2c0-4b71-a4c3-1f3b8f8e0a11"
}
```

## License

MIT
