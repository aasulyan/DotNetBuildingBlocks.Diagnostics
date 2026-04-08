using DotNetBuildingBlocks.Metrics.Internal;

namespace DotNetBuildingBlocks.Metrics.Extensions;

/// <summary>
/// Helpful extensions for histogram recording.
/// </summary>
public static class HistogramExtensions
{
    public static void RecordDuration(
        this Histogram<double> histogram,
        TimeSpan elapsed,
        params KeyValuePair<string, object?>[] tags)
    {
        ArgumentGuard.NotNull(histogram, nameof(histogram));
        histogram.Record(elapsed.TotalMilliseconds, tags);
    }

    public static void RecordDuration(
        this Histogram<long> histogram,
        TimeSpan elapsed,
        params KeyValuePair<string, object?>[] tags)
    {
        ArgumentGuard.NotNull(histogram, nameof(histogram));
        histogram.Record((long)Math.Round(elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero), tags);
    }
}
