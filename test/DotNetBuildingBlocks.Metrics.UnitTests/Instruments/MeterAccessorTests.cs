using DotNetBuildingBlocks.Metrics.Instruments;
using DotNetBuildingBlocks.Metrics.Options;
using FluentAssertions;
using Xunit;

namespace DotNetBuildingBlocks.Metrics.UnitTests.Instruments;

public sealed class MeterAccessorTests
{
    [Fact]
    public void Constructor_Should_Create_Meter_With_Configured_Name_And_Version()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MetricsOptions
        {
            MeterName = "Samples.OrderProcessor",
            MeterVersion = "1.2.3"
        });

        using var accessor = new MeterAccessor(options);

        accessor.Meter.Name.Should().Be("Samples.OrderProcessor");
        accessor.Meter.Version.Should().Be("1.2.3");
    }

    [Fact]
    public void CreateCounter_Should_Throw_For_Invalid_Name()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MetricsOptions { MeterName = "Samples.OrderProcessor" });
        using var accessor = new MeterAccessor(options);

        var action = () => accessor.CreateCounter<long>(" ");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateHistogram_Should_Return_Usable_Instrument()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MetricsOptions { MeterName = "Samples.OrderProcessor" });
        using var accessor = new MeterAccessor(options);

        var histogram = accessor.CreateHistogram<double>("operation.duration", "ms", "Operation duration.");

        histogram.Should().NotBeNull();
        histogram.Name.Should().Be("operation.duration");
        histogram.Unit.Should().Be("ms");
        histogram.Description.Should().Be("Operation duration.");
    }

    [Fact]
    public void CreateObservableGauge_Should_Throw_When_Callback_Is_Null()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MetricsOptions { MeterName = "Samples.OrderProcessor" });
        using var accessor = new MeterAccessor(options);

        var action = () => accessor.CreateObservableGauge<int>("queue.depth", null!);

        action.Should().Throw<ArgumentNullException>();
    }
}
