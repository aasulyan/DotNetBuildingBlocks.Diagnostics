# DotNetBuildingBlocks.Logging

Provider-agnostic structured logging helpers for .NET applications.

## Purpose

`DotNetBuildingBlocks.Logging` helps applications and libraries write logs in a more consistent way by providing:

- structured scope helpers
- common logging event identifiers
- reusable logger extension methods
- provider-agnostic registration helpers

This package builds on top of `Microsoft.Extensions.Logging` and does not configure any concrete logging provider.

## What this package provides

- `BeginPropertyScope(...)`
- `BeginCorrelationScope(...)`
- `BeginOperationScope(...)`
- `BeginEntityScope(...)`
- `LogOperationStarted(...)`
- `LogOperationCompleted(...)`
- `LogOperationFailed(...)`
- `LogValidationFailed(...)`
- `LogUnhandledException(...)`
- `LogRetryAttempt(...)`
- `LoggingEventIds`

## What this package does not provide

- Serilog configuration
- OpenTelemetry setup
- ASP.NET Core middleware
- request/response logging middleware
- sinks, exporters, or storage integrations

## Installation

```bash
dotnet add package DotNetBuildingBlocks.Logging
