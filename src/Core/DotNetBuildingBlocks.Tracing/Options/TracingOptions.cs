namespace DotNetBuildingBlocks.Tracing.Options;

/// <summary>
/// Configures package-owned tracing services.
/// </summary>
public sealed class TracingOptions
{
    /// <summary>
    /// Gets or sets the activity source name used by the package-owned <see cref="ActivitySource"/>.
    /// </summary>
    public string ActivitySourceName { get; set; } = "DotNetBuildingBlocks";

    /// <summary>
    /// Gets or sets the optional activity source version.
    /// </summary>
    public string? ActivitySourceVersion { get; set; }
}
