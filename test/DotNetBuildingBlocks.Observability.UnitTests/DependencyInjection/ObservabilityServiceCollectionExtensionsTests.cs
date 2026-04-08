using DotNetBuildingBlocks.Metrics.Abstractions;
using DotNetBuildingBlocks.Observation.Models;
using DotNetBuildingBlocks.Observation.Options;
using DotNetBuildingBlocks.Observability.DependencyInjection;
using DotNetBuildingBlocks.Observability.Options;
using DotNetBuildingBlocks.Tracing.Abstractions;
using DotNetBuildingBlocks.Tracing.Options;
using DotNetBuildingBlocks.Metrics.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNetBuildingBlocks.Observability.UnitTests.DependencyInjection;

public sealed class ObservabilityServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Register_ObservationIdentity_And_Map_Names()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
            options.ServiceVersion = "1.0.0";
        });

        using var provider = services.BuildServiceProvider();

        var identity = provider.GetRequiredService<ObservationIdentity>();
        var tracingOptions = provider.GetRequiredService<IOptions<TracingOptions>>().Value;
        var metricsOptions = provider.GetRequiredService<IOptions<MetricsOptions>>().Value;

        identity.ServiceName.Should().Be("Orders.Service");
        identity.ActivitySourceName.Should().Be("Orders.Service");
        identity.MeterName.Should().Be("Orders.Service");
        identity.ServiceVersion.Should().Be("1.0.0");

        tracingOptions.ActivitySourceName.Should().Be("Orders.Service");
        tracingOptions.ActivitySourceVersion.Should().Be("1.0.0");

        metricsOptions.MeterName.Should().Be("Orders.Service");
        metricsOptions.MeterVersion.Should().Be("1.0.0");
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Use_Explicit_ActivitySource_And_Meter_Names()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
            options.ActivitySourceName = "Orders.Tracing";
            options.MeterName = "Orders.Metrics";
        });

        using var provider = services.BuildServiceProvider();

        var identity = provider.GetRequiredService<ObservationIdentity>();
        identity.ActivitySourceName.Should().Be("Orders.Tracing");
        identity.MeterName.Should().Be("Orders.Metrics");
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Register_Tracing_And_Metrics_By_Default()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
        });

        using var provider = services.BuildServiceProvider();

        provider.GetService<IActivitySourceAccessor>().Should().NotBeNull();
        provider.GetService<IMeterAccessor>().Should().NotBeNull();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Allow_Disabling_Tracing()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
            options.EnableTracing = false;
        });

        using var provider = services.BuildServiceProvider();

        provider.GetService<IActivitySourceAccessor>().Should().BeNull();
        provider.GetService<IMeterAccessor>().Should().NotBeNull();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Allow_Disabling_Metrics()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
            options.EnableMetrics = false;
        });

        using var provider = services.BuildServiceProvider();

        provider.GetService<IActivitySourceAccessor>().Should().NotBeNull();
        provider.GetService<IMeterAccessor>().Should().BeNull();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Throw_When_ServiceName_Is_Missing()
    {
        var services = new ServiceCollection();

        var act = () => services.AddDotNetBuildingBlocksObservability();

        act.Should().Throw<ArgumentException>()
            .WithMessage("*ServiceName*");
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Be_Idempotent_For_Core_Identity_Service()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
        });

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
        });

        services.Count(d => d.ServiceType == typeof(ObservationIdentity)).Should().Be(1);
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Register_ObservabilityOptions()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
            options.ServiceVersion = "2.0.0";
            options.EnableLogging = true;
            options.Exporters.Otlp.Enabled = true;
            options.Exporters.Otlp.Endpoint = "http://localhost:4317";
            options.Instrumentations.AspNetCore.Enabled = true;
            options.Instrumentations.HttpClient.Enabled = true;
        });

        using var provider = services.BuildServiceProvider();

        var resolvedOptions = provider.GetRequiredService<IOptions<ObservabilityOptions>>().Value;

        resolvedOptions.ServiceName.Should().Be("Orders.Service");
        resolvedOptions.ServiceVersion.Should().Be("2.0.0");
        resolvedOptions.EnableLogging.Should().BeTrue();
        resolvedOptions.Exporters.Otlp.Enabled.Should().BeTrue();
        resolvedOptions.Exporters.Otlp.Endpoint.Should().Be("http://localhost:4317");
        resolvedOptions.Instrumentations.AspNetCore.Enabled.Should().BeTrue();
        resolvedOptions.Instrumentations.HttpClient.Enabled.Should().BeTrue();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Should_Configure_Resource_Options()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksObservability(options =>
        {
            options.ServiceName = "Orders.Service";
            options.Resource.ServiceNamespace = "orders";
            options.Resource.ServiceInstanceId = "pod-abc-123";
            options.Resource.DeploymentEnvironment = "staging";
            options.Resource.Attributes["team"] = "platform";
        });

        using var provider = services.BuildServiceProvider();

        var resolvedOptions = provider.GetRequiredService<IOptions<ObservabilityOptions>>().Value;

        resolvedOptions.Resource.ServiceNamespace.Should().Be("orders");
        resolvedOptions.Resource.ServiceInstanceId.Should().Be("pod-abc-123");
        resolvedOptions.Resource.DeploymentEnvironment.Should().Be("staging");
        resolvedOptions.Resource.Attributes.Should().ContainKey("team");
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Defaults_Should_Have_Tracing_And_Metrics_Enabled()
    {
        var options = new ObservabilityOptions();

        options.EnableTracing.Should().BeTrue();
        options.EnableMetrics.Should().BeTrue();
        options.EnableLogging.Should().BeFalse();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Defaults_Should_Have_All_Instrumentations_Disabled()
    {
        var options = new ObservabilityOptions();

        options.Instrumentations.AspNetCore.Enabled.Should().BeFalse();
        options.Instrumentations.HttpClient.Enabled.Should().BeFalse();
        options.Instrumentations.Runtime.Enabled.Should().BeFalse();
        options.Instrumentations.Process.Enabled.Should().BeFalse();
    }

    [Fact]
    public void AddDotNetBuildingBlocksObservability_Defaults_Should_Have_Otlp_Disabled()
    {
        var options = new ObservabilityOptions();

        options.Exporters.Otlp.Enabled.Should().BeFalse();
        options.Exporters.Otlp.Endpoint.Should().BeNull();
        options.Exporters.Otlp.Protocol.Should().BeNull();
        options.Exporters.Otlp.Headers.Should().BeEmpty();
    }
}
