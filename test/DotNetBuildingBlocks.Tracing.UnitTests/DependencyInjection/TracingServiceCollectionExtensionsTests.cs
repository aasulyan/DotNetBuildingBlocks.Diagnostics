using Microsoft.Extensions.Options;

namespace DotNetBuildingBlocks.Tracing.UnitTests.DependencyInjection;

public sealed class TracingServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDotNetBuildingBlocksTracing_Should_Register_Services()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksTracing(options =>
        {
            options.ActivitySourceName = "Tests.Tracing";
            options.ActivitySourceVersion = "2.0.0";
        });

        using var provider = services.BuildServiceProvider();

        var accessor = provider.GetRequiredService<IActivitySourceAccessor>();
        var options = provider.GetRequiredService<IOptions<TracingOptions>>();

        accessor.Should().NotBeNull();
        accessor.ActivitySource.Name.Should().Be("Tests.Tracing");
        accessor.ActivitySource.Version.Should().Be("2.0.0");
        options.Value.ActivitySourceName.Should().Be("Tests.Tracing");
        options.Value.ActivitySourceVersion.Should().Be("2.0.0");
    }

    [Fact]
    public void AddDotNetBuildingBlocksTracing_Should_Register_Accessor_As_Singleton()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksTracing(options =>
        {
            options.ActivitySourceName = "Tests.Tracing";
        });

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IActivitySourceAccessor>();
        var second = provider.GetRequiredService<IActivitySourceAccessor>();

        first.Should().BeSameAs(second);
    }

    [Fact]
    public void AddDotNetBuildingBlocksTracing_Should_Throw_When_Services_Is_Null()
    {
        ServiceCollection? services = null;

        Action act = () => services!.AddDotNetBuildingBlocksTracing();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    [Fact]
    public void AddDotNetBuildingBlocksTracing_Should_Allow_Default_Options()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksTracing();

        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<TracingOptions>>();
        var accessor = provider.GetRequiredService<IActivitySourceAccessor>();

        options.Value.ActivitySourceName.Should().Be("DotNetBuildingBlocks");
        options.Value.ActivitySourceVersion.Should().BeNull();
        accessor.ActivitySource.Name.Should().Be("DotNetBuildingBlocks");
    }

    [Fact]
    public void AddDotNetBuildingBlocksTracing_Should_Validate_Options_When_Resolved()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksTracing(options =>
        {
            options.ActivitySourceName = " ";
        });

        using var provider = services.BuildServiceProvider();

        Action act = () => _ = provider.GetRequiredService<IActivitySourceAccessor>();

        act.Should().Throw<OptionsValidationException>()
            .Which.Failures.Should().Contain("TracingOptions.ActivitySourceName cannot be null or whitespace.");
    }
}
