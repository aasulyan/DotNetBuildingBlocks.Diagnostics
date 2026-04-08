namespace DotNetBuildingBlocks.Observation.Models;

/// <summary>
/// Represents the resolved observation identity used by logging, tracing, and metrics.
/// </summary>
/// <param name="ServiceName">The logical service name.</param>
/// <param name="ServiceVersion">The optional service version.</param>
/// <param name="ActivitySourceName">The resolved activity source name.</param>
/// <param name="MeterName">The resolved meter name.</param>
public sealed record ObservationIdentity(
    string ServiceName,
    string? ServiceVersion,
    string ActivitySourceName,
    string MeterName);
