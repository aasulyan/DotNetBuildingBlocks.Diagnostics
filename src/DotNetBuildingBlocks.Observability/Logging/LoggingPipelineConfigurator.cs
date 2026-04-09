using DotNetBuildingBlocks.Observability.Options;

namespace DotNetBuildingBlocks.Observability.Logging;

/// <summary>
/// Configures the logging pipeline for the observability layer.
/// <para>
/// The generic <c>DotNetBuildingBlocks.Logging</c> helpers are composed via
/// the <c>DotNetBuildingBlocks.Observation</c> layer, so no OpenTelemetry log
/// pipeline is registered here yet. This configurator exists as the extension
/// point for future OpenTelemetry log export wiring (OpenTelemetryLoggerProvider).
/// </para>
/// </summary>
internal static class LoggingPipelineConfigurator
{
    /// <summary>
    /// Reserved hook for future OpenTelemetry logging pipeline configuration.
    /// Currently a no-op because logging is composed through the Observation layer
    /// (which registers <c>DotNetBuildingBlocks.Logging</c>).
    /// </summary>
    public static void Configure(ObservabilityOptions options)
    {
        // Intentionally empty. Extension point for future OpenTelemetry logs pipeline.
        _ = options;
    }
}
