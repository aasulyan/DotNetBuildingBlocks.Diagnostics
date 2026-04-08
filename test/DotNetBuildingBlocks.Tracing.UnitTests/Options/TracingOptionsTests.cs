namespace DotNetBuildingBlocks.Tracing.UnitTests.Options;

public sealed class TracingOptionsTests
{
    [Fact]
    public void TracingOptions_Should_Have_Expected_Defaults()
    {
        var options = new TracingOptions();

        options.ActivitySourceName.Should().Be("DotNetBuildingBlocks");
        options.ActivitySourceVersion.Should().BeNull();
    }
}
