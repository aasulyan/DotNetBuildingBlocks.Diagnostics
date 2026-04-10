using DotNetBuildingBlocks.Serilog.Internal;

namespace DotNetBuildingBlocks.Serilog.Enrichers;

/// <summary>
/// Enriches log events with <c>TraceId</c>, <c>SpanId</c>, and <c>ParentSpanId</c> properties
/// read from <see cref="Activity.Current"/>.
/// </summary>
/// <remarks>
/// When no current activity exists the enricher does nothing. Existing properties of the same
/// name are preserved.
/// </remarks>
public sealed class ActivityEnricher : ILogEventEnricher
{
    /// <summary>
    /// The well-known property name for the trace identifier.
    /// </summary>
    public const string TraceIdPropertyName = "TraceId";

    /// <summary>
    /// The well-known property name for the span identifier.
    /// </summary>
    public const string SpanIdPropertyName = "SpanId";

    /// <summary>
    /// The well-known property name for the parent span identifier.
    /// </summary>
    public const string ParentSpanIdPropertyName = "ParentSpanId";

    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentGuard.ThrowIfNull(logEvent, nameof(logEvent));
        ArgumentGuard.ThrowIfNull(propertyFactory, nameof(propertyFactory));

        var activity = Activity.Current;
        if (activity is null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            TraceIdPropertyName,
            activity.TraceId.ToString()));

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            SpanIdPropertyName,
            activity.SpanId.ToString()));

        if (activity.ParentSpanId != default)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                ParentSpanIdPropertyName,
                activity.ParentSpanId.ToString()));
        }
    }
}
