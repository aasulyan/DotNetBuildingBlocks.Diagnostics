using DotNetBuildingBlocks.Observation.DependencyInjection;
using DotNetBuildingBlocks.Observability.Internal;
using DotNetBuildingBlocks.Observability.Logging;
using DotNetBuildingBlocks.Observability.Metrics;
using DotNetBuildingBlocks.Observability.Options;
using DotNetBuildingBlocks.Observability.Tracing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenTelemetry;

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

        // Defer validation errors to the options validation layer.
        // If ServiceName is missing, skip eager pipeline bootstrap;
        // the OptionsValidationException will fire when IOptions<ObservabilityOptions>.Value is accessed.
        if (string.IsNullOrWhiteSpace(bootstrapOptions.ServiceName))
        {
            return services;
        }

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

        // Delegate pipeline configuration to folder-based configurators.
        var otelBuilder = services.AddOpenTelemetry();

        if (bootstrapOptions.EnableTracing)
        {
            otelBuilder.WithTracing(tracing =>
                tracing.ConfigureDotNetBuildingBlocksTracing(bootstrapOptions, resourceBuilder, activitySourceName));
        }

        if (bootstrapOptions.EnableMetrics)
        {
            otelBuilder.WithMetrics(metrics =>
                metrics.ConfigureDotNetBuildingBlocksMetrics(bootstrapOptions, resourceBuilder, meterName));
        }

        if (bootstrapOptions.EnableLogging)
        {
            services.ConfigureDotNetBuildingBlocksLogging(bootstrapOptions);
        }

        return services;
    }
}
