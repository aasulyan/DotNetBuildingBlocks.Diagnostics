# DotNetBuildingBlocks.Tracing

`DotNetBuildingBlocks.Tracing` is a small reusable package for creating and enriching `System.Diagnostics.Activity` traces in a consistent way.

It helps you:

- create activities from one shared `ActivitySource`
- use simple helper methods for common activity kinds
- enrich activities with common tags
- mark success and failure in a consistent way
- capture current trace context for propagation to other processes or messages
- register tracing services with dependency injection

This package is provider-agnostic.  
It does not force OpenTelemetry SDK, exporters, ASP.NET Core middleware, or any specific vendor.

---

## Why this package exists

In many projects, teams want tracing but do not want tracing code spread everywhere in different styles.

Without a shared package, one service may:
- create raw `Activity` objects manually
- use different tag names
- forget to mark errors
- use different correlation id patterns
- make context propagation harder

This package gives one simple common layer for tracing inside your solutions.

---

## Main features

- `IActivitySourceAccessor`
- `ActivitySourceAccessor`
- `TracingOptions`
- `ActivityExtensions`
- `TracingTagNames`
- `TraceContextSnapshot`
- `AddDotNetBuildingBlocksTracing(...)`

---

## Installation

```bash
dotnet add package DotNetBuildingBlocks.Tracing
