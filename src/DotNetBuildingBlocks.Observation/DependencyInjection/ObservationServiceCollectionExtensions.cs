using DotNetBuildingBlocks.Logging.DependencyInjection;
using DotNetBuildingBlocks.Metrics.DependencyInjection;
using DotNetBuildingBlocks.Metrics.Options;
using DotNetBuildingBlocks.Observation.Internal;
using DotNetBuildingBlocks.Observation.Options;
using DotNetBuildingBlocks.Tracing.DependencyInjection;
using DotNetBuildingBlocks.Tracing.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DotNetBuildingBlocks.Observation.DependencyInjection;

/// <summary>
/// Provides dependency injection extensions for registering the observation package.
/// </summary>
public static class ObservationServiceCollectionExtensions
{
    /// <summary>
    /// Registers observation services and composes logging, tracing, and metrics packages.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An optional delegate used to configure <see cref="ObservationOptions" />.</param>
    /// <returns>The same <see cref="IServiceCollection" /> instance.</returns>
    public static IServiceCollection AddDotNetBuildingBlocksObservation(
        this IServiceCollection services,
        Action<ObservationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<ObservationOptions>()
            .Configure(options => configure?.Invoke(options))
            .ValidateDataAnnotations()
            .Validate(
                static options => !string.IsNullOrWhiteSpace(options.ServiceName),
                "ObservationOptions.ServiceName cannot be null or whitespace.")
            .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<ObservationOptions>, ObservationOptionsValidator>();
        services.TryAddSingleton(static serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ObservationOptions>>().Value;
            return ObservationIdentityResolver.Resolve(options);
        });

        services.AddOptions<TracingOptions>()
            .Configure<IOptions<ObservationOptions>>((tracingOptions, observationOptionsAccessor) =>
            {
                var identity = ObservationIdentityResolver.Resolve(observationOptionsAccessor.Value);
                tracingOptions.ActivitySourceName = identity.ActivitySourceName;
                tracingOptions.ActivitySourceVersion = identity.ServiceVersion;
            });

        services.AddOptions<MetricsOptions>()
            .Configure<IOptions<ObservationOptions>>((metricsOptions, observationOptionsAccessor) =>
            {
                var identity = ObservationIdentityResolver.Resolve(observationOptionsAccessor.Value);
                metricsOptions.MeterName = identity.MeterName;
                metricsOptions.MeterVersion = identity.ServiceVersion;
            });

        var bootstrapOptions = new ObservationOptions();
        configure?.Invoke(bootstrapOptions);
        var bootstrapIdentity = ObservationIdentityResolver.Resolve(bootstrapOptions);

        if (bootstrapOptions.ConfigureLogging)
        {
            services.AddDotNetBuildingBlocksLogging();
        }

        if (bootstrapOptions.ConfigureTracing)
        {
            services.AddDotNetBuildingBlocksTracing(options =>
            {
                options.ActivitySourceName = bootstrapIdentity.ActivitySourceName;
                options.ActivitySourceVersion = bootstrapIdentity.ServiceVersion;
            });
        }

        if (bootstrapOptions.ConfigureMetrics)
        {
            services.AddDotNetBuildingBlocksMetrics(options =>
            {
                options.MeterName = bootstrapIdentity.MeterName;
                options.MeterVersion = bootstrapIdentity.ServiceVersion;
            });
        }

        return services;
    }

    private sealed class ObservationOptionsValidator : IValidateOptions<ObservationOptions>
    {
        public ValidateOptionsResult Validate(string? name, ObservationOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            return string.IsNullOrWhiteSpace(options.ServiceName)
                ? ValidateOptionsResult.Fail("ObservationOptions.ServiceName cannot be null or whitespace.")
                : ValidateOptionsResult.Success;
        }
    }
}
