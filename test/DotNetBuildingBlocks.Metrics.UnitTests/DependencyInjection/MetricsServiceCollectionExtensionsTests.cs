using DotNetBuildingBlocks.Metrics.Abstractions;
using DotNetBuildingBlocks.Metrics.DependencyInjection;
using DotNetBuildingBlocks.Metrics.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNetBuildingBlocks.Metrics.UnitTests.DependencyInjection;

public sealed class MetricsServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDotNetBuildingBlocksMetrics_Should_Register_Services()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksMetrics(options =>
        {
            options.MeterName = "Samples.OrderProcessor";
            options.MeterVersion = "1.0.0";
        });

        using var provider = services.BuildServiceProvider();

        var accessor = provider.GetRequiredService<IMeterAccessor>();
        var options = provider.GetRequiredService<IOptions<MetricsOptions>>();

        accessor.Meter.Name.Should().Be("Samples.OrderProcessor");
        accessor.Meter.Version.Should().Be("1.0.0");
        options.Value.MeterName.Should().Be("Samples.OrderProcessor");
    }

    [Fact]
    public void AddDotNetBuildingBlocksMetrics_Should_Be_Idempotent()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksMetrics(options => options.MeterName = "Samples.OrderProcessor");
        services.AddDotNetBuildingBlocksMetrics();

        using var provider = services.BuildServiceProvider();
        var accessors = provider.GetServices<IMeterAccessor>();

        accessors.Should().ContainSingle();
    }

    [Fact]
    public void AddDotNetBuildingBlocksMetrics_Should_Validate_Options_When_Resolved()
    {
        var services = new ServiceCollection();
        services.AddDotNetBuildingBlocksMetrics(options => options.MeterName = " ");

        using var provider = services.BuildServiceProvider();

        var action = () => provider.GetRequiredService<IMeterAccessor>();

        action.Should().Throw<OptionsValidationException>()
            .WithMessage("*MetricsOptions.MeterName cannot be null or whitespace.*");
    }
}
