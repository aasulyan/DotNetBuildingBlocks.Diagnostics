using DotNetBuildingBlocks.Observability.Internal;
using DotNetBuildingBlocks.Observability.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotNetBuildingBlocks.Observability.Tracing;

/// <summary>
/// Provides fluent extension methods for configuring the DotNetBuildingBlocks
/// OpenTelemetry tracing pipeline on a <see cref="TracerProviderBuilder"/>.
/// </summary>
internal static class TracerProviderBuilderExtensions
{
    /// <summary>
    /// Configures the tracing pipeline with the DotNetBuildingBlocks observability
    /// resource, activity source, instrumentations, and exporters.
    /// </summary>
    public static TracerProviderBuilder ConfigureDotNetBuildingBlocksTracing(
        this TracerProviderBuilder builder,
        ObservabilityOptions options,
        ResourceBuilder resourceBuilder,
        string activitySourceName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(resourceBuilder);
        ArgumentException.ThrowIfNullOrWhiteSpace(activitySourceName);

        builder.SetResourceBuilder(resourceBuilder);
        builder.AddSource(activitySourceName);

        builder.AddConfiguredInstrumentations(options.Instrumentations);
        builder.AddConfiguredExporters(options.Exporters);

        return builder;
    }

    private static TracerProviderBuilder AddConfiguredInstrumentations(
        this TracerProviderBuilder builder,
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

        return builder;
    }

    private static TracerProviderBuilder AddConfiguredExporters(
        this TracerProviderBuilder builder,
        ObservabilityExporterOptions exporters)
    {
        if (exporters.Otlp.Enabled)
        {
            builder.AddOtlpExporter(otlp => OtlpExporterConfigurator.Apply(otlp, exporters.Otlp));
        }

        return builder;
    }
}
