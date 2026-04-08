namespace DotNetBuildingBlocks.Observability.Options;

/// <summary>
/// Options for configuring OpenTelemetry exporters.
/// </summary>
public sealed class ObservabilityExporterOptions
{
    /// <summary>
    /// OTLP exporter configuration.
    /// </summary>
    public OtlpOptions Otlp { get; } = new();
}
