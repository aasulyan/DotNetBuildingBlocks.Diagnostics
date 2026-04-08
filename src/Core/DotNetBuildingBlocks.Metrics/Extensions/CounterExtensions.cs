using DotNetBuildingBlocks.Metrics.Internal;
using DotNetBuildingBlocks.Metrics.Tags;

namespace DotNetBuildingBlocks.Metrics.Extensions;

/// <summary>
/// Helpful extensions for counter recording.
/// </summary>
public static class CounterExtensions
{
    public static void Increment(this Counter<long> counter, params KeyValuePair<string, object?>[] tags)
    {
        ArgumentGuard.NotNull(counter, nameof(counter));
        counter.Add(1, tags);
    }

    public static void Increment(this Counter<int> counter, params KeyValuePair<string, object?>[] tags)
    {
        ArgumentGuard.NotNull(counter, nameof(counter));
        counter.Add(1, tags);
    }

    public static void IncrementSuccess(this Counter<long> counter, params KeyValuePair<string, object?>[] tags)
    {
        ArgumentGuard.NotNull(counter, nameof(counter));
        counter.Add(1, Append(tags, MetricTags.Outcome("success")));
    }

    public static void IncrementFailure(this Counter<long> counter, params KeyValuePair<string, object?>[] tags)
    {
        ArgumentGuard.NotNull(counter, nameof(counter));
        counter.Add(1, Append(tags, MetricTags.Outcome("failure")));
    }

    private static KeyValuePair<string, object?>[] Append(
        KeyValuePair<string, object?>[]? tags,
        KeyValuePair<string, object?> tag)
    {
        if (tags is null || tags.Length == 0)
        {
            return [tag];
        }

        var combined = new KeyValuePair<string, object?>[tags.Length + 1];
        Array.Copy(tags, combined, tags.Length);
        combined[^1] = tag;
        return combined;
    }
}
