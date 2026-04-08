using DotNetBuildingBlocks.Metrics.Extensions;
using DotNetBuildingBlocks.Metrics.Instruments;
using DotNetBuildingBlocks.Metrics.Options;
using DotNetBuildingBlocks.Metrics.Tags;
using DotNetBuildingBlocks.Metrics.UnitTests.TestUtilities;
using FluentAssertions;
using Xunit;

namespace DotNetBuildingBlocks.Metrics.UnitTests.Extensions;

public sealed class CounterExtensionsTests
{
    [Fact]
    public void IncrementSuccess_Should_Add_Success_Outcome_Tag()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MetricsOptions { MeterName = "Samples.OrderProcessor" });
        using var accessor = new MeterAccessor(options);
        using var collector = new MetricCollector<long>(accessor.Meter, "requests.total");

        var counter = accessor.CreateCounter<long>("requests.total", description: "Processed requests.");

        counter.IncrementSuccess(MetricTags.Operation("CreateOrder"));

        collector.Measurements.Should().ContainSingle();
        collector.Measurements[0].Value.Should().Be(1L);
        collector.Measurements[0].Tags.Should().Contain(x => x.Key == MetricTagNames.Operation && Equals(x.Value, "CreateOrder"));
        collector.Measurements[0].Tags.Should().Contain(x => x.Key == MetricTagNames.Outcome && Equals(x.Value, "success"));
    }

    [Fact]
    public void IncrementFailure_Should_Add_Failure_Outcome_Tag()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MetricsOptions { MeterName = "Samples.OrderProcessor" });
        using var accessor = new MeterAccessor(options);
        using var collector = new MetricCollector<long>(accessor.Meter, "requests.total");

        var counter = accessor.CreateCounter<long>("requests.total");

        counter.IncrementFailure(MetricTags.Operation("CreateOrder"));

        collector.Measurements.Should().ContainSingle();
        collector.Measurements[0].Tags.Should().Contain(x => x.Key == MetricTagNames.Outcome && Equals(x.Value, "failure"));
    }
}
