namespace DotNetBuildingBlocks.Serilog.UnitTests.Options;

public sealed class DotNetSerilogOptionsTests
{
    [Fact]
    public void Defaults_Should_Be_Sensible()
    {
        var options = new DotNetSerilogOptions();

        options.ApplicationName.Should().Be("DotNetBuildingBlocks");
        options.ApplicationVersion.Should().BeNull();
        options.MinimumLevel.Should().Be(LogEventLevel.Information);
        options.UseConsole.Should().BeTrue();
        options.UseDebug.Should().BeFalse();
        options.UseJsonConsoleFormatter.Should().BeFalse();
        options.IncludeMachineName.Should().BeTrue();
        options.IncludeEnvironmentName.Should().BeTrue();
        options.IncludeThreadId.Should().BeFalse();
        options.IncludeActivityEnricher.Should().BeTrue();
        options.IncludeCorrelationEnricher.Should().BeTrue();
        options.ReadFromConfiguration.Should().BeTrue();
        options.ConfigurationSectionName.Should().Be("Serilog");
    }

    [Fact]
    public void Explicit_Values_Should_Be_Preserved()
    {
        var options = new DotNetSerilogOptions
        {
            ApplicationName = "Tests.Serilog",
            ApplicationVersion = "1.2.3",
            MinimumLevel = LogEventLevel.Debug,
            UseConsole = false,
            UseDebug = true,
            UseJsonConsoleFormatter = true,
            IncludeMachineName = false,
            IncludeEnvironmentName = false,
            IncludeThreadId = true,
            IncludeActivityEnricher = false,
            IncludeCorrelationEnricher = false,
            ReadFromConfiguration = false,
            ConfigurationSectionName = "CustomSerilog"
        };

        options.ApplicationName.Should().Be("Tests.Serilog");
        options.ApplicationVersion.Should().Be("1.2.3");
        options.MinimumLevel.Should().Be(LogEventLevel.Debug);
        options.UseConsole.Should().BeFalse();
        options.UseDebug.Should().BeTrue();
        options.UseJsonConsoleFormatter.Should().BeTrue();
        options.IncludeMachineName.Should().BeFalse();
        options.IncludeEnvironmentName.Should().BeFalse();
        options.IncludeThreadId.Should().BeTrue();
        options.IncludeActivityEnricher.Should().BeFalse();
        options.IncludeCorrelationEnricher.Should().BeFalse();
        options.ReadFromConfiguration.Should().BeFalse();
        options.ConfigurationSectionName.Should().Be("CustomSerilog");
    }
}
