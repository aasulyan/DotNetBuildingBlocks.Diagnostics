using DotNetBuildingBlocks.Metrics.Abstractions;
using DotNetBuildingBlocks.Metrics.Internal;
using DotNetBuildingBlocks.Metrics.Options;
using Microsoft.Extensions.Options;

namespace DotNetBuildingBlocks.Metrics.Instruments;

/// <summary>
/// Default implementation of <see cref="IMeterAccessor"/>.
/// </summary>
public sealed class MeterAccessor : IMeterAccessor, IDisposable
{
    private readonly Meter _meter;
    private bool _disposed;

    public MeterAccessor(IOptions<MetricsOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var metricsOptions = options.Value;
        var meterName = ArgumentGuard.NotNullOrWhiteSpace(metricsOptions.MeterName, nameof(metricsOptions.MeterName));

        _meter = new Meter(meterName, metricsOptions.MeterVersion);
    }

    public Meter Meter
    {
        get
        {
            ThrowIfDisposed();
            return _meter;
        }
    }

    public Counter<T> CreateCounter<T>(string name, string? unit = null, string? description = null)
        where T : struct
    {
        ThrowIfDisposed();
        return _meter.CreateCounter<T>(ValidateInstrumentName(name), unit, description);
    }

    public UpDownCounter<T> CreateUpDownCounter<T>(string name, string? unit = null, string? description = null)
        where T : struct
    {
        ThrowIfDisposed();
        return _meter.CreateUpDownCounter<T>(ValidateInstrumentName(name), unit, description);
    }

    public Histogram<T> CreateHistogram<T>(string name, string? unit = null, string? description = null)
        where T : struct
    {
        ThrowIfDisposed();
        return _meter.CreateHistogram<T>(ValidateInstrumentName(name), unit, description);
    }

    public ObservableGauge<T> CreateObservableGauge<T>(string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(observeValue);
        return _meter.CreateObservableGauge(ValidateInstrumentName(name), observeValue, unit, description);
    }

    public ObservableCounter<T> CreateObservableCounter<T>(string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(observeValue);
        return _meter.CreateObservableCounter(ValidateInstrumentName(name), observeValue, unit, description);
    }

    public ObservableUpDownCounter<T> CreateObservableUpDownCounter<T>(string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(observeValue);
        return _meter.CreateObservableUpDownCounter(ValidateInstrumentName(name), observeValue, unit, description);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _meter.Dispose();
        _disposed = true;
    }

    private static string ValidateInstrumentName(string name)
        => ArgumentGuard.NotNullOrWhiteSpace(name, nameof(name));

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
