namespace DotNetBuildingBlocks.Metrics.Options;

/// <summary>
/// Configures the shared <see cref="Meter"/> used by the package.
/// </summary>
public sealed class MetricsOptions
{
    /// <summary>
    /// Gets or sets the meter name.
    /// </summary>
    public string MeterName { get; set; } = "DotNetBuildingBlocks";

    /// <summary>
    /// Gets or sets the optional meter version.
    /// </summary>
    public string? MeterVersion { get; set; }
}
