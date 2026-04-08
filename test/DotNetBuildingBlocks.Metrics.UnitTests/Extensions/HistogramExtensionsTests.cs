using System.Diagnostics.Metrics;
using DotNetBuildingBlocks.Metrics.Extensions;
using DotNetBuildingBlocks.Metrics.Instruments;
using DotNetBuildingBlocks.Metrics.Options;
using DotNetBuildingBlocks.Metrics.Tags;
using DotNetBuildingBlocks.Metrics.UnitTests.TestUtilities;
using FluentAssertions;
using Xunit;

namespace DotNetBuildingBlocks.Metrics.UnitTests.Extensions;

public sealed class HistogramExtensionsTests
{
    [Fact]
    public void RecordDuration_Should_Record_TotalMilliseconds_And_Tags()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MetricsOptions { MeterName = "Samples.OrderProcessor" });
        using var accessor = new MeterAccessor(options);
        using var collector = new MetricCollector<double>(accessor.Meter, "operation.duration");

        var histogram = accessor.CreateHistogram<double>("operation.duration", "ms", "Operation duration.");

        histogram.RecordDuration(
            TimeSpan.FromMilliseconds(125.5),
            MetricTags.Operation("ProcessOrder"),
            MetricTags.Outcome("success"));

        collector.Measurements.Should().ContainSingle();
        collector.Measurements[0].Value.Should().Be(125.5d);
        collector.Measurements[0].Tags.Should().Contain(x => x.Key == MetricTagNames.Operation && Equals(x.Value, "ProcessOrder"));
        collector.Measurements[0].Tags.Should().Contain(x => x.Key == MetricTagNames.Outcome && Equals(x.Value, "success"));
    }

    [Fact]
    public void RecordDuration_Should_Throw_When_Histogram_Is_Null()
    {
        Histogram<double> histogram = null!;

        var action = () => histogram.RecordDuration(TimeSpan.FromMilliseconds(10));

        action.Should().Throw<ArgumentNullException>();
    }
}
