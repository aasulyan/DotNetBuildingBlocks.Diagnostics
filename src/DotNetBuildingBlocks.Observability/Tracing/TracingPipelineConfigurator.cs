using DotNetBuildingBlocks.Observability.Internal;
using DotNetBuildingBlocks.Observability.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotNetBuildingBlocks.Observability.Tracing;

/// <summary>
/// Configures the OpenTelemetry tracing pipeline:
/// activity source registration, instrumentations, and exporters.
/// </summary>
internal static class TracingPipelineConfigurator
{
    public static void Configure(
        TracerProviderBuilder tracing,
        ObservabilityOptions options,
        ResourceBuilder resourceBuilder,
        string activitySourceName)
    {
        tracing.SetResourceBuilder(resourceBuilder);
        tracing.AddSource(activitySourceName);

        ConfigureInstrumentations(tracing, options.Instrumentations);
        ConfigureExporters(tracing, options.Exporters);
    }

    private static void ConfigureInstrumentations(
        TracerProviderBuilder tracing,
        ObservabilityInstrumentationOptions instrumentations)
    {
        if (instrumentations.AspNetCore.Enabled)
        {
            tracing.AddAspNetCoreInstrumentation();
        }

        if (instrumentations.HttpClient.Enabled)
        {
            tracing.AddHttpClientInstrumentation();
        }
    }

    private static void ConfigureExporters(
        TracerProviderBuilder tracing,
        ObservabilityExporterOptions exporters)
    {
        if (exporters.Otlp.Enabled)
        {
            tracing.AddOtlpExporter(otlp => OtlpExporterConfigurator.Apply(otlp, exporters.Otlp));
        }
    }
}
