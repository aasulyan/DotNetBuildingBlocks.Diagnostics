namespace DotNetBuildingBlocks.Metrics.Abstractions;

/// <summary>
/// Provides access to a configured <see cref="Meter"/> instance and creates instruments consistently.
/// </summary>
public interface IMeterAccessor
{
    /// <summary>
    /// Gets the configured <see cref="Meter"/> instance.
    /// </summary>
    Meter Meter { get; }

    /// <summary>
    /// Creates a <see cref="Counter{T}"/> instrument.
    /// </summary>
    Counter<T> CreateCounter<T>(string name, string? unit = null, string? description = null)
        where T : struct;

    /// <summary>
    /// Creates an <see cref="UpDownCounter{T}"/> instrument.
    /// </summary>
    UpDownCounter<T> CreateUpDownCounter<T>(string name, string? unit = null, string? description = null)
        where T : struct;

    /// <summary>
    /// Creates a <see cref="Histogram{T}"/> instrument.
    /// </summary>
    Histogram<T> CreateHistogram<T>(string name, string? unit = null, string? description = null)
        where T : struct;

    /// <summary>
    /// Creates an <see cref="ObservableGauge{T}"/> instrument.
    /// </summary>
    ObservableGauge<T> CreateObservableGauge<T>(string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct;

    /// <summary>
    /// Creates an <see cref="ObservableCounter{T}"/> instrument.
    /// </summary>
    ObservableCounter<T> CreateObservableCounter<T>(string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct;

    /// <summary>
    /// Creates an <see cref="ObservableUpDownCounter{T}"/> instrument.
    /// </summary>
    ObservableUpDownCounter<T> CreateObservableUpDownCounter<T>(string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct;
}
