using DotNetBuildingBlocks.Observability.Options;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuildingBlocks.Observability.Logging;

/// <summary>
/// Provides extension methods for configuring the DotNetBuildingBlocks
/// OpenTelemetry logging pipeline on an <see cref="IServiceCollection"/>.
/// <para>
/// The generic <c>DotNetBuildingBlocks.Logging</c> helpers are composed via
/// the <c>DotNetBuildingBlocks.Observation</c> layer, so no OpenTelemetry log
/// pipeline is registered here yet. This extension exists as the public hook
/// for future OpenTelemetry log export wiring (OpenTelemetryLoggerProvider).
/// </para>
/// </summary>
internal static class ObservabilityLoggingServiceCollectionExtensions
{
    /// <summary>
    /// Reserved hook for future OpenTelemetry logging pipeline configuration.
    /// Currently a no-op because logging is composed through the Observation layer
    /// (which registers <c>DotNetBuildingBlocks.Logging</c>).
    /// </summary>
    public static IServiceCollection ConfigureDotNetBuildingBlocksLogging(
        this IServiceCollection services,
        ObservabilityOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        // Intentionally empty. Extension point for future OpenTelemetry logs pipeline.
        return services;
    }
}
