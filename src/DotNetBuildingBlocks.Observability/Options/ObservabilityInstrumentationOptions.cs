namespace DotNetBuildingBlocks.Observability.Options;

/// <summary>
/// Options for toggling OpenTelemetry instrumentations.
/// </summary>
public sealed class ObservabilityInstrumentationOptions
{
    /// <summary>
    /// ASP.NET Core request/response instrumentation.
    /// </summary>
    public InstrumentationToggle AspNetCore { get; } = new();

    /// <summary>
    /// HttpClient outgoing request instrumentation.
    /// </summary>
    public InstrumentationToggle HttpClient { get; } = new();

    /// <summary>
    /// .NET runtime instrumentation (GC, thread pool, JIT).
    /// </summary>
    public InstrumentationToggle Runtime { get; } = new();

    /// <summary>
    /// Process-level instrumentation (CPU, memory, threads).
    /// </summary>
    public InstrumentationToggle Process { get; } = new();
}
