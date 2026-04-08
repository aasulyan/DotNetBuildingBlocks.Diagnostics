using System.Diagnostics.Metrics;

namespace DotNetBuildingBlocks.Metrics.UnitTests.TestUtilities;

internal sealed class MetricCollector<T> : IDisposable
    where T : struct
{
    private readonly MeterListener _listener;
    private readonly List<CollectedMeasurement<T>> _measurements = [];

    public MetricCollector(Meter meter, string instrumentName)
    {
        _listener = new MeterListener
        {
            InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == meter.Name && instrument.Name == instrumentName)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            }
        };

        _listener.SetMeasurementEventCallback<T>((instrument, measurement, tags, state) =>
        {
            var capturedTags = new List<KeyValuePair<string, object?>>();
            foreach (var tag in tags)
            {
                capturedTags.Add(tag);
            }

            _measurements.Add(new CollectedMeasurement<T>(instrument.Name, measurement, capturedTags));
        });

        _listener.Start();
    }

    public IReadOnlyList<CollectedMeasurement<T>> Measurements => _measurements;

    public void Dispose() => _listener.Dispose();
}

internal sealed record CollectedMeasurement<T>(
    string InstrumentName,
    T Value,
    IReadOnlyList<KeyValuePair<string, object?>> Tags)
    where T : struct;
