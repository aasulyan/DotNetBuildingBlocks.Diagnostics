using DotNetBuildingBlocks.Logging.Context;
using DotNetBuildingBlocks.Logging.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNetBuildingBlocks.Logging.UnitTests.DependencyInjection;

public sealed class LoggingServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDotNetBuildingBlocksLogging_Should_Register_Package_Services()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksLogging();

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ILogScopeStateFactory>().Should().NotBeNull();
    }

    [Fact]
    public void AddDotNetBuildingBlocksLogging_Should_Be_Idempotent()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksLogging();
        services.AddDotNetBuildingBlocksLogging();

        services.Count(x => x.ServiceType == typeof(ILogScopeStateFactory)).Should().Be(1);
    }

    [Fact]
    public void AddDotNetBuildingBlocksLogging_Should_Throw_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var action = services.AddDotNetBuildingBlocksLogging;

        action.Should().Throw<ArgumentNullException>();
    }
}
