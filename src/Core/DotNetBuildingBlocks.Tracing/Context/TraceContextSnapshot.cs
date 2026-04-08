using System.Collections.ObjectModel;
using DotNetBuildingBlocks.Tracing.Internal;
using DotNetBuildingBlocks.Tracing.Tags;

namespace DotNetBuildingBlocks.Tracing.Context;

/// <summary>
/// Represents a small immutable snapshot of the current trace context.
/// </summary>
/// <param name="TraceId">The current trace id.</param>
/// <param name="SpanId">The current span id.</param>
/// <param name="TraceParent">The current traceparent value.</param>
/// <param name="TraceState">The current tracestate value.</param>
/// <param name="CorrelationId">The current correlation id when available.</param>
public sealed record TraceContextSnapshot(
    string TraceId,
    string SpanId,
    string? TraceParent,
    string? TraceState,
    string? CorrelationId)
{
    /// <summary>
    /// Captures the current activity context.
    /// </summary>
    /// <returns>A snapshot when <see cref="Activity.Current"/> exists; otherwise <see langword="null"/>.</returns>
    public static TraceContextSnapshot? CaptureCurrent()
    {
        var activity = Activity.Current;
        return activity is null ? null : FromActivity(activity);
    }

    /// <summary>
    /// Tries to capture the current trace context.
    /// </summary>
    /// <param name="snapshot">The captured snapshot, when available.</param>
    /// <returns><see langword="true"/> when a current activity exists; otherwise <see langword="false"/>.</returns>
    public static bool TryCaptureCurrent(out TraceContextSnapshot? snapshot)
    {
        snapshot = CaptureCurrent();
        return snapshot is not null;
    }

    /// <summary>
    /// Converts the snapshot into a propagation-friendly header dictionary.
    /// </summary>
    /// <returns>A read-only dictionary of propagation header values.</returns>
    public IReadOnlyDictionary<string, string> ToHeaders()
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(TraceParent))
        {
            headers["traceparent"] = TraceParent;
        }

        if (!string.IsNullOrWhiteSpace(TraceState))
        {
            headers["tracestate"] = TraceState;
        }

        if (!string.IsNullOrWhiteSpace(CorrelationId))
        {
            headers[TracingTagNames.CorrelationId] = CorrelationId;
        }

        return new ReadOnlyDictionary<string, string>(headers);
    }

    /// <summary>
    /// Creates a snapshot from an explicit <see cref="Activity"/>.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>A new snapshot instance.</returns>
    public static TraceContextSnapshot FromActivity(Activity activity)
    {
        ArgumentGuard.ThrowIfNull(activity, nameof(activity));

        var correlationId = activity.GetTagItem(TracingTagNames.CorrelationId)?.ToString();

        return new TraceContextSnapshot(
            activity.TraceId.ToString(),
            activity.SpanId.ToString(),
            activity.Id,
            activity.TraceStateString,
            correlationId);
    }
}
