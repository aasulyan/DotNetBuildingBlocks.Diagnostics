using DotNetBuildingBlocks.Observation.DependencyInjection;
using DotNetBuildingBlocks.Observability.Internal;
using DotNetBuildingBlocks.Observability.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace DotNetBuildingBlocks.Observability.DependencyInjection;

/// <summary>
/// Provides dependency injection extensions for registering the OpenTelemetry-based observability pipeline.
/// </summary>
public static class ObservabilityServiceCollectionExtensions
{
    /// <summary>
    /// Registers the DotNetBuildingBlocks observability pipeline including OpenTelemetry SDK,
    /// resource metadata, instrumentations, and exporters.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">A delegate used to configure <see cref="ObservabilityOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddDotNetBuildingBlocksObservability(
        this IServiceCollection services,
        Action<ObservabilityOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register and validate options.
        services.AddOptions<ObservabilityOptions>()
            .Configure(options => configure?.Invoke(options))
            .ValidateDataAnnotations()
            .Validate(
                static options => !string.IsNullOrWhiteSpace(options.ServiceName),
                "ObservabilityOptions.ServiceName cannot be null or whitespace.")
            .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<ObservabilityOptions>, ObservabilityOptionsValidator>();

        // Bootstrap options eagerly for pipeline configuration.
        var bootstrapOptions = new ObservabilityOptions();
        configure?.Invoke(bootstrapOptions);

        var serviceName = bootstrapOptions.ServiceName.Trim();
        var activitySourceName = string.IsNullOrWhiteSpace(bootstrapOptions.ActivitySourceName)
            ? serviceName
            : bootstrapOptions.ActivitySourceName.Trim();
        var meterName = string.IsNullOrWhiteSpace(bootstrapOptions.MeterName)
            ? serviceName
            : bootstrapOptions.MeterName.Trim();

        // Compose the generic Observation layer (Logging + Tracing + Metrics registration).
        services.AddDotNetBuildingBlocksObservation(observationOptions =>
        {
            observationOptions.ServiceName = serviceName;
            observationOptions.ServiceVersion = bootstrapOptions.ServiceVersion;
            observationOptions.ActivitySourceName = activitySourceName;
            observationOptions.MeterName = meterName;
            observationOptions.ConfigureLogging = bootstrapOptions.EnableLogging;
            observationOptions.ConfigureTracing = bootstrapOptions.EnableTracing;
            observationOptions.ConfigureMetrics = bootstrapOptions.EnableMetrics;
        });

        // Build the OpenTelemetry resource.
        var resourceBuilder = ResourceBuilderFactory.Create(bootstrapOptions, activitySourceName, meterName);

        // Register OpenTelemetry SDK pipelines.
        var otelBuilder = services.AddOpenTelemetry();

        if (bootstrapOptions.EnableTracing)
        {
            otelBuilder.WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(resourceBuilder);
                tracing.AddSource(activitySourceName);

                ConfigureTracingInstrumentations(tracing, bootstrapOptions.Instrumentations);
                ConfigureTracingExporters(tracing, bootstrapOptions.Exporters);
            });
        }

        if (bootstrapOptions.EnableMetrics)
        {
            otelBuilder.WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(resourceBuilder);
                metrics.AddMeter(meterName);

                ConfigureMetricsInstrumentations(metrics, bootstrapOptions.Instrumentations);
                ConfigureMetricsExporters(metrics, bootstrapOptions.Exporters);
            });
        }

        return services;
    }

    private static void ConfigureTracingInstrumentations(
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

    private static void ConfigureTracingExporters(
        TracerProviderBuilder tracing,
        ObservabilityExporterOptions exporters)
    {
        if (exporters.Otlp.Enabled)
        {
            tracing.AddOtlpExporter(otlp => ApplyOtlpOptions(otlp, exporters.Otlp));
        }
    }

    private static void ConfigureMetricsInstrumentations(
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

    private static void ConfigureMetricsExporters(
        MeterProviderBuilder metrics,
        ObservabilityExporterOptions exporters)
    {
        if (exporters.Otlp.Enabled)
        {
            metrics.AddOtlpExporter(otlp => ApplyOtlpOptions(otlp, exporters.Otlp));
        }
    }

    private static void ApplyOtlpOptions(
        OpenTelemetry.Exporter.OtlpExporterOptions otlp,
        OtlpOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            otlp.Endpoint = new Uri(options.Endpoint);
        }

        if (!string.IsNullOrWhiteSpace(options.Protocol))
        {
            otlp.Protocol = options.Protocol.Equals("grpc", StringComparison.OrdinalIgnoreCase)
                ? OpenTelemetry.Exporter.OtlpExportProtocol.Grpc
                : OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
        }

        if (options.Headers.Count > 0)
        {
            otlp.Headers = string.Join(",", options.Headers.Select(h => $"{h.Key}={h.Value}"));
        }
    }
}
