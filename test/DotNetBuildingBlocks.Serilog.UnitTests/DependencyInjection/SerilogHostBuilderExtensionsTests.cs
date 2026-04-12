using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetBuildingBlocks.Serilog.UnitTests.DependencyInjection;

public sealed class SerilogHostBuilderExtensionsTests
{
    [Fact]
    public void UseDotNetBuildingBlocksSerilog_Should_Throw_When_HostBuilder_Is_Null()
    {
        IHostBuilder? hostBuilder = null;

        Action act = () => hostBuilder!.UseDotNetBuildingBlocksSerilog();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("hostBuilder");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void UseDotNetBuildingBlocksSerilog_Should_Throw_When_ApplicationName_Is_Invalid(string? applicationName)
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        Action act = () => hostBuilder.UseDotNetBuildingBlocksSerilog(o => o.ApplicationName = applicationName!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UseDotNetBuildingBlocksSerilog_Should_Build_Host_And_Resolve_Logger()
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .UseDotNetBuildingBlocksSerilog(options =>
            {
                options.ApplicationName = "Tests.Serilog";
                options.ApplicationVersion = "1.0.0";
                options.UseConsole = false;
                options.ReadFromConfiguration = false;
            });

        using var host = hostBuilder.Build();

        var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("UnitTests");

        logger.Should().NotBeNull();

        Action act = () => logger.LogInformation("Bootstrapped {Component}", "Serilog");
        act.Should().NotThrow();
    }

    [Fact]
    public void UseDotNetBuildingBlocksSerilog_Should_Resolve_Generic_ILogger_From_Host()
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .UseDotNetBuildingBlocksSerilog(options =>
            {
                options.ApplicationName = "Tests.Serilog";
                options.ApplicationVersion = "1.0.0";
                options.UseConsole = false;
                options.ReadFromConfiguration = false;
            });

        using var host = hostBuilder.Build();

        var logger = host.Services.GetRequiredService<ILogger<SerilogHostBuilderExtensionsTests>>();

        logger.Should().NotBeNull();

        Action act = () => logger.LogInformation("Hello {Subject}", "world");
        act.Should().NotThrow();
    }
}
