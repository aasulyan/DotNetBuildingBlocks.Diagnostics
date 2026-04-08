using DotNetBuildingBlocks.Metrics.Tags;
using FluentAssertions;
using Xunit;

namespace DotNetBuildingBlocks.Metrics.UnitTests.Tags;

public sealed class MetricTagsTests
{
    [Fact]
    public void Operation_Should_Return_Expected_Tag()
    {
        var tag = MetricTags.Operation("CreateOrder");

        tag.Key.Should().Be(MetricTagNames.Operation);
        tag.Value.Should().Be("CreateOrder");
    }

    [Fact]
    public void EntityId_Should_Use_Stable_Tag_Name()
    {
        var tag = MetricTags.EntityId(42);

        tag.Key.Should().Be(MetricTagNames.EntityId);
        tag.Value.Should().Be(42);
    }

    [Fact]
    public void TenantId_Should_Throw_When_Value_Is_Whitespace()
    {
        var action = () => MetricTags.TenantId(" ");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Should_Allow_Custom_Tag_Name()
    {
        var tag = MetricTags.Create("custom.dimension", "value");

        tag.Key.Should().Be("custom.dimension");
        tag.Value.Should().Be("value");
    }
}
