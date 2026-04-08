namespace DotNetBuildingBlocks.Observability.Options;

/// <summary>
/// Options for configuring OpenTelemetry resource metadata attributes.
/// </summary>
public sealed class ObservabilityResourceOptions
{
    /// <summary>
    /// The optional service namespace (e.g. "orders", "payments").
    /// </summary>
    public string? ServiceNamespace { get; set; }

    /// <summary>
    /// A unique identifier for this service instance (e.g. pod name, container id).
    /// </summary>
    public string? ServiceInstanceId { get; set; }

    /// <summary>
    /// The deployment environment name (e.g. "production", "staging", "development").
    /// </summary>
    public string? DeploymentEnvironment { get; set; }

    /// <summary>
    /// Additional resource attributes to include in the OpenTelemetry resource.
    /// </summary>
    public IDictionary<string, object> Attributes { get; } = new Dictionary<string, object>();
}
