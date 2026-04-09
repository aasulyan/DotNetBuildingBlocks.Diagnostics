using DotNetBuildingBlocks.Observability.Internal;
using DotNetBuildingBlocks.Observability.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace DotNetBuildingBlocks.Observability.Metrics;

/// <summary>
/// Configures the OpenTelemetry metrics pipeline:
/// meter registration, instrumentations, and exporters.
/// </summary>
internal static class MetricsPipelineConfigurator
{
    public static void Configure(
        MeterProviderBuilder metrics,
        ObservabilityOptions options,
        ResourceBuilder resourceBuilder,
        string meterName)
    {
        metrics.SetResourceBuilder(resourceBuilder);
        metrics.AddMeter(meterName);

        ConfigureInstrumentations(metrics, options.Instrumentations);
        ConfigureExporters(metrics, options.Exporters);
    }

    private static void ConfigureInstrumentations(
        MeterProviderBuilder metrics,
        ObservabilityInstrumentationOptions instrumentations)
    {
        if (instrumentations.AspNetCore.Enabled)
        {
            metrics.AddAspNetCoreInstrumentation();
        }

        if (instrumentations.HttpClient.Enabled)
        {
            metrics.AddHttpClientInstrumentation();
        }

        if (instrumentations.Runtime.Enabled)
        {
            metrics.AddRuntimeInstrumentation();
        }

        if (instrumentations.Process.Enabled)
        {
            metrics.AddProcessInstrumentation();
        }
    }

    private static void ConfigureExporters(
        MeterProviderBuilder metrics,
        ObservabilityExporterOptions exporters)
    {
        if (exporters.Otlp.Enabled)
        {
            metrics.AddOtlpExporter(otlp => OtlpExporterConfigurator.Apply(otlp, exporters.Otlp));
        }
    }
}
