using DotNetBuildingBlocks.Metrics.Abstractions;
using DotNetBuildingBlocks.Metrics.Options;
using DotNetBuildingBlocks.Observation.DependencyInjection;
using DotNetBuildingBlocks.Observation.Models;
using DotNetBuildingBlocks.Observation.Options;
using DotNetBuildingBlocks.Tracing.Abstractions;
using DotNetBuildingBlocks.Tracing.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNetBuildingBlocks.Observation.UnitTests.DependencyInjection;

public sealed class ObservationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDotNetBuildingBlocksObservation_Should_Register_ObservationIdentity_And_Map_Names()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservation(options =>
        {
            options.ServiceName = "Samples.OrderProcessor";
            options.ServiceVersion = "1.2.3";
        });

        using var provider = services.BuildServiceProvider();

        var identity = provider.GetRequiredService<ObservationIdentity>();
        var tracingOptions = provider.GetRequiredService<IOptions<TracingOptions>>().Value;
        var metricsOptions = provider.GetRequiredService<IOptions<MetricsOptions>>().Value;
        var observationOptions = provider.GetRequiredService<IOptions<ObservationOptions>>().Value;

        identity.ServiceName.Should().Be("Samples.OrderProcessor");
        identity.ActivitySourceName.Should().Be("Samples.OrderProcessor");
        identity.MeterName.Should().Be("Samples.OrderProcessor");
        identity.ServiceVersion.Should().Be("1.2.3");

        tracingOptions.ActivitySourceName.Should().Be("Samples.OrderProcessor");
        tracingOptions.ActivitySourceVersion.Should().Be("1.2.3");

        metricsOptions.MeterName.Should().Be("Samples.OrderProcessor");
        metricsOptions.MeterVersion.Should().Be("1.2.3");

        observationOptions.ConfigureLogging.Should().BeTrue();
        observationOptions.ConfigureTracing.Should().BeTrue();
        observationOptions.ConfigureMetrics.Should().BeTrue();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservation_Should_Use_Explicit_Tracing_And_Metrics_Names_When_Provided()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservation(options =>
        {
            options.ServiceName = "Samples.OrderProcessor";
            options.ServiceVersion = "2.0.0";
            options.ActivitySourceName = "Samples.OrderProcessor.Tracing";
            options.MeterName = "Samples.OrderProcessor.Metrics";
        });

        using var provider = services.BuildServiceProvider();

        var identity = provider.GetRequiredService<ObservationIdentity>();
        var tracingOptions = provider.GetRequiredService<IOptions<TracingOptions>>().Value;
        var metricsOptions = provider.GetRequiredService<IOptions<MetricsOptions>>().Value;

        identity.ActivitySourceName.Should().Be("Samples.OrderProcessor.Tracing");
        identity.MeterName.Should().Be("Samples.OrderProcessor.Metrics");
        tracingOptions.ActivitySourceName.Should().Be("Samples.OrderProcessor.Tracing");
        metricsOptions.MeterName.Should().Be("Samples.OrderProcessor.Metrics");
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservation_Should_Register_Tracing_And_Metrics_Services_By_Default()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservation(options =>
        {
            options.ServiceName = "Samples.OrderProcessor";
        });

        using var provider = services.BuildServiceProvider();

        provider.GetService<IActivitySourceAccessor>().Should().NotBeNull();
        provider.GetService<IMeterAccessor>().Should().NotBeNull();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservation_Should_Allow_Disabling_Tracing_And_Metrics()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservation(options =>
        {
            options.ServiceName = "Samples.OrderProcessor";
            options.ConfigureTracing = false;
            options.ConfigureMetrics = false;
        });

        using var provider = services.BuildServiceProvider();

        provider.GetService<IActivitySourceAccessor>().Should().BeNull();
        provider.GetService<IMeterAccessor>().Should().BeNull();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservation_Should_Be_Idempotent_For_Core_Identity_Service()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservation(options =>
        {
            options.ServiceName = "Samples.OrderProcessor";
        });

        services.AddDotNetBuildingBlocksObservation(options =>
        {
            options.ServiceName = "Samples.OrderProcessor";
        });

        services.Count(descriptor => descriptor.ServiceType == typeof(ObservationIdentity)).Should().Be(1);
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservation_Should_Throw_When_ServiceName_Is_Missing()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservation();

        using var provider = services.BuildServiceProvider();

        var act = () => provider.GetRequiredService<IOptions<ObservationOptions>>().Value;

        act.Should().Throw<OptionsValidationException>()
            .WithMessage("*ServiceName*");
    }
}
