namespace DotNetBuildingBlocks.Observability.Options;

/// <summary>
/// Options for configuring the OTLP exporter.
/// </summary>
public sealed class OtlpOptions
{
    /// <summary>
    /// Whether the OTLP exporter is enabled. Default is <c>false</c>.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The OTLP collector endpoint (e.g. "http://localhost:4317").
    /// When null, the OpenTelemetry SDK default is used.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// The OTLP transport protocol. Supported values: "grpc", "http/protobuf".
    /// When null, the OpenTelemetry SDK default is used.
    /// </summary>
    public string? Protocol { get; set; }

    /// <summary>
    /// Additional headers to include in OTLP export requests.
    /// </summary>
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
}
