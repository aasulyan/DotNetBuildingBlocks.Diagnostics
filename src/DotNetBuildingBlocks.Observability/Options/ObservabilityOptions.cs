using System.ComponentModel.DataAnnotations;

namespace DotNetBuildingBlocks.Observability.Options;

/// <summary>
/// Root options for configuring the DotNetBuildingBlocks observability pipeline,
/// including OpenTelemetry SDK setup, instrumentations, and exporters.
/// </summary>
public sealed class ObservabilityOptions
{
    /// <summary>
    /// The logical service name used for resource metadata, tracing, and metrics.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// The service version used for resource metadata and instrument versioning.
    /// </summary>
    public string? ServiceVersion { get; set; }

    /// <summary>
    /// Enables the OpenTelemetry tracing pipeline. Default is <c>true</c>.
    /// </summary>
    public bool EnableTracing { get; set; } = true;

    /// <summary>
    /// Enables the OpenTelemetry metrics pipeline. Default is <c>true</c>.
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Enables the OpenTelemetry logging pipeline. Default is <c>false</c>.
    /// </summary>
    public bool EnableLogging { get; set; }

    /// <summary>
    /// Overrides the default ActivitySource name. When null, defaults to <see cref="ServiceName"/>.
    /// </summary>
    public string? ActivitySourceName { get; set; }

    /// <summary>
    /// Overrides the default Meter name. When null, defaults to <see cref="ServiceName"/>.
    /// </summary>
    public string? MeterName { get; set; }

    /// <summary>
    /// Resource metadata options for the OpenTelemetry resource builder.
    /// </summary>
    public ObservabilityResourceOptions Resource { get; } = new();

    /// <summary>
    /// Instrumentation toggle options.
    /// </summary>
    public ObservabilityInstrumentationOptions Instrumentations { get; } = new();

    /// <summary>
    /// Exporter configuration options.
    /// </summary>
    public ObservabilityExporterOptions Exporters { get; } = new();
}
