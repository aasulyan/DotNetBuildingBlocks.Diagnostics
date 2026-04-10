using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetBuildingBlocks.Serilog.UnitTests.DependencyInjection;

public sealed class SerilogServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDotNetBuildingBlocksSerilog_Should_Throw_When_Services_Is_Null()
    {
        IServiceCollection? services = null;

        Action act = () => services!.AddDotNetBuildingBlocksSerilog();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void AddDotNetBuildingBlocksSerilog_Should_Throw_When_ApplicationName_Is_Invalid(string? applicationName)
    {
        var services = new ServiceCollection();

        Action act = () => services.AddDotNetBuildingBlocksSerilog(o => o.ApplicationName = applicationName!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddDotNetBuildingBlocksSerilog_Should_Register_Logger_Factory()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksSerilog(options =>
        {
            options.ApplicationName = "Tests.Serilog";
            options.UseConsole = false;
            options.ReadFromConfiguration = false;
        });

        using var provider = services.BuildServiceProvider();

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("UnitTests");

        logger.Should().NotBeNull();

        Action act = () => logger.LogInformation("Hello {Subject}", "DI");
        act.Should().NotThrow();
    }

    [Fact]
    public void AddDotNetBuildingBlocksSerilog_Should_Resolve_Generic_ILogger()
    {
        var services = new ServiceCollection();

        services.AddDotNetBuildingBlocksSerilog(options =>
        {
            options.ApplicationName = "Tests.Serilog";
            options.UseConsole = false;
            options.ReadFromConfiguration = false;
        });

        using var provider = services.BuildServiceProvider();

        var logger = provider.GetRequiredService<ILogger<SerilogServiceCollectionExtensionsTests>>();
        logger.Should().NotBeNull();
    }
}
