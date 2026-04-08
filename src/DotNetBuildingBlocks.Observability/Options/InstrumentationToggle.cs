namespace DotNetBuildingBlocks.Observability.Options;

/// <summary>
/// A simple toggle for enabling or disabling an instrumentation.
/// </summary>
public sealed class InstrumentationToggle
{
    /// <summary>
    /// Whether this instrumentation is enabled. Default is <c>false</c>.
    /// </summary>
    public bool Enabled { get; set; }
}
