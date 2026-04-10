using DotNetBuildingBlocks.Observability.Internal;
using DotNetBuildingBlocks.Observability.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace DotNetBuildingBlocks.Observability.Metrics;

/// <summary>
/// Provides fluent extension methods for configuring the DotNetBuildingBlocks
/// OpenTelemetry metrics pipeline on a <see cref="MeterProviderBuilder"/>.
/// </summary>
internal static class MeterProviderBuilderExtensions
{
    /// <summary>
    /// Configures the metrics pipeline with the DotNetBuildingBlocks observability
    /// resource, meter, instrumentations, and exporters.
    /// </summary>
    public static MeterProviderBuilder ConfigureDotNetBuildingBlocksMetrics(
        this MeterProviderBuilder builder,
        ObservabilityOptions options,
        ResourceBuilder resourceBuilder,
        string meterName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(resourceBuilder);
        ArgumentException.ThrowIfNullOrWhiteSpace(meterName);

        builder.SetResourceBuilder(resourceBuilder);
        builder.AddMeter(meterName);

        builder.AddConfiguredInstrumentations(options.Instrumentations);
        builder.AddConfiguredExporters(options.Exporters);

        return builder;
    }

    private static MeterProviderBuilder AddConfiguredInstrumentations(
        this MeterProviderBuilder builder,
        ObservabilityInstrumentationOptions instrumentations)
    {
        if (instrumentations.AspNetCore.Enabled)
        {
            builder.AddAspNetCoreInstrumentation();
        }

        if (instrumentations.HttpClient.Enabled)
        {
            builder.AddHttpClientInstrumentation();
        }

        if (instrumentations.Runtime.Enabled)
        {
            builder.AddRuntimeInstrumentation();
        }

        if (instrumentations.Process.Enabled)
        {
            builder.AddProcessInstrumentation();
        }

        return builder;
    }

    private static MeterProviderBuilder AddConfiguredExporters(
        this MeterProviderBuilder builder,
        ObservabilityExporterOptions exporters)
    {
        if (exporters.Otlp.Enabled)
        {
            builder.AddOtlpExporter(otlp => OtlpExporterConfigurator.Apply(otlp, exporters.Otlp));
        }

        return builder;
    }
}
