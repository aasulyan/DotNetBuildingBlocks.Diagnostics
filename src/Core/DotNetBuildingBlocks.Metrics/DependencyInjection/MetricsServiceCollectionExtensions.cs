using DotNetBuildingBlocks.Metrics.Abstractions;
using DotNetBuildingBlocks.Metrics.Instruments;
using DotNetBuildingBlocks.Metrics.Internal;
using DotNetBuildingBlocks.Metrics.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DotNetBuildingBlocks.Metrics.DependencyInjection;

/// <summary>
/// Registers DotNetBuildingBlocks metrics services.
/// </summary>
public static class MetricsServiceCollectionExtensions
{
    public static IServiceCollection AddDotNetBuildingBlocksMetrics(
        this IServiceCollection services,
        Action<MetricsOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<MetricsOptions>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<MetricsOptions>, MetricsOptionsValidator>());

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IMeterAccessor, MeterAccessor>();

        return services;
    }
}
