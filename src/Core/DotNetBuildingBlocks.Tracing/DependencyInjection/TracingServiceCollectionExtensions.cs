using DotNetBuildingBlocks.Tracing.Abstractions;
using DotNetBuildingBlocks.Tracing.Activities;
using DotNetBuildingBlocks.Tracing.Internal;
using DotNetBuildingBlocks.Tracing.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetBuildingBlocks.Tracing.DependencyInjection;

/// <summary>
/// Service registration extensions for DotNetBuildingBlocks tracing services.
/// </summary>
public static class TracingServiceCollectionExtensions
{
    /// <summary>
    /// Registers DotNetBuildingBlocks tracing services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional options configuration.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddDotNetBuildingBlocksTracing(
        this IServiceCollection services,
        Action<TracingOptions>? configure = null)
    {
        ArgumentGuard.ThrowIfNull(services, nameof(services));

        services.AddOptions<TracingOptions>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            Microsoft.Extensions.Options.IValidateOptions<TracingOptions>,
            TracingOptionsValidator>());

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IActivitySourceAccessor, ActivitySourceAccessor>();

        return services;
    }
}
