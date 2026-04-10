namespace DotNetBuildingBlocks.Serilog.UnitTests.Configuration;

public sealed class LoggerConfigurationExtensionsTests
{
    public LoggerConfigurationExtensionsTests()
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
    }

    [Fact]
    public void ApplyDotNetBuildingBlocksDefaults_Should_Throw_When_LoggerConfiguration_Is_Null()
    {
        LoggerConfiguration? loggerConfiguration = null;

        Action act = () => loggerConfiguration!.ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
        {
            ApplicationName = "Tests"
        });

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("loggerConfiguration");
    }

    [Fact]
    public void ApplyDotNetBuildingBlocksDefaults_Should_Throw_When_Options_Is_Null()
    {
        var loggerConfiguration = new LoggerConfiguration();

        Action act = () => loggerConfiguration.ApplyDotNetBuildingBlocksDefaults(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ApplyDotNetBuildingBlocksDefaults_Should_Throw_When_ApplicationName_Is_Invalid(string? applicationName)
    {
        var loggerConfiguration = new LoggerConfiguration();
        var options = new DotNetSerilogOptions
        {
            ApplicationName = applicationName!
        };

        Action act = () => loggerConfiguration.ApplyDotNetBuildingBlocksDefaults(options);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ApplyDefaults_Should_Add_Application_Identity_Properties()
    {
        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
            {
                ApplicationName = "Tests.Serilog",
                ApplicationVersion = "1.2.3",
                UseConsole = false,
                IncludeMachineName = false,
                IncludeEnvironmentName = false,
                IncludeActivityEnricher = false,
                IncludeCorrelationEnricher = false
            })
            .CreateLogger();

        logger.Information("Hello");

        var logEvent = sink.Single();
        logEvent.Properties[ApplicationIdentityEnricher.ApplicationNamePropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("Tests.Serilog");
        logEvent.Properties[ApplicationIdentityEnricher.ApplicationVersionPropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("1.2.3");
    }

    [Fact]
    public void ApplyDefaults_Should_Add_MachineName_When_Enabled()
    {
        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
            {
                ApplicationName = "Tests.Serilog",
                UseConsole = false,
                IncludeMachineName = true,
                IncludeEnvironmentName = false,
                IncludeActivityEnricher = false,
                IncludeCorrelationEnricher = false
            })
            .CreateLogger();

        logger.Information("Hello");

        var logEvent = sink.Single();
        logEvent.Properties.Should().ContainKey("MachineName");
        logEvent.Properties["MachineName"].Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be(Environment.MachineName);
    }

    [Fact]
    public void ApplyDefaults_Should_Add_EnvironmentName_From_Argument_When_Enabled()
    {
        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(
                new DotNetSerilogOptions
                {
                    ApplicationName = "Tests.Serilog",
                    UseConsole = false,
                    IncludeMachineName = false,
                    IncludeEnvironmentName = true,
                    IncludeActivityEnricher = false,
                    IncludeCorrelationEnricher = false
                },
                environmentName: "Staging")
            .CreateLogger();

        logger.Information("Hello");

        var logEvent = sink.Single();
        logEvent.Properties["EnvironmentName"].Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("Staging");
    }

    [Fact]
    public void ApplyDefaults_Should_Skip_EnvironmentName_When_Disabled()
    {
        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(
                new DotNetSerilogOptions
                {
                    ApplicationName = "Tests.Serilog",
                    UseConsole = false,
                    IncludeMachineName = false,
                    IncludeEnvironmentName = false,
                    IncludeActivityEnricher = false,
                    IncludeCorrelationEnricher = false
                },
                environmentName: "Staging")
            .CreateLogger();

        logger.Information("Hello");

        var logEvent = sink.Single();
        logEvent.Properties.Should().NotContainKey("EnvironmentName");
    }

    [Fact]
    public void ApplyDefaults_Should_Add_Activity_Properties_When_Enricher_Enabled()
    {
        using var activity = new Activity("logger-config-test").Start();

        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
            {
                ApplicationName = "Tests.Serilog",
                UseConsole = false,
                IncludeMachineName = false,
                IncludeEnvironmentName = false,
                IncludeActivityEnricher = true,
                IncludeCorrelationEnricher = false
            })
            .CreateLogger();

        logger.Information("Hello");

        var logEvent = sink.Single();
        logEvent.Properties.Should().ContainKey(ActivityEnricher.TraceIdPropertyName);
        logEvent.Properties.Should().ContainKey(ActivityEnricher.SpanIdPropertyName);
    }

    [Fact]
    public void ApplyDefaults_Should_Apply_Minimum_Level()
    {
        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
            {
                ApplicationName = "Tests.Serilog",
                MinimumLevel = LogEventLevel.Warning,
                UseConsole = false,
                IncludeMachineName = false,
                IncludeEnvironmentName = false,
                IncludeActivityEnricher = false,
                IncludeCorrelationEnricher = false
            })
            .CreateLogger();

        logger.Information("filtered");
        logger.Warning("captured");

        sink.Events.Should().ContainSingle()
            .Which.Level.Should().Be(LogEventLevel.Warning);
    }

    [Fact]
    public void ApplyDefaults_Should_Add_ThreadId_When_Enabled()
    {
        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
            {
                ApplicationName = "Tests.Serilog",
                UseConsole = false,
                IncludeMachineName = false,
                IncludeEnvironmentName = false,
                IncludeActivityEnricher = false,
                IncludeCorrelationEnricher = false,
                IncludeThreadId = true
            })
            .CreateLogger();

        logger.Information("Hello");

        var logEvent = sink.Single();
        logEvent.Properties.Should().ContainKey("ThreadId");
    }

    [Fact]
    public void ApplyDefaults_Should_Promote_CorrelationId_From_Baggage_When_Missing()
    {
        Activity.Current = null;
        using var activity = new Activity("with-baggage").Start();
        activity.AddBaggage("CorrelationId", "abc-123");

        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
            {
                ApplicationName = "Tests.Serilog",
                UseConsole = false,
                IncludeMachineName = false,
                IncludeEnvironmentName = false,
                IncludeActivityEnricher = false,
                IncludeCorrelationEnricher = true
            })
            .CreateLogger();

        logger.Information("Hello");

        var logEvent = sink.Single();
        logEvent.Properties.Should().ContainKey("CorrelationId");
        logEvent.Properties["CorrelationId"].Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("abc-123");
    }

    [Fact]
    public void ApplyDefaults_Should_Preserve_LogContext_CorrelationId_Over_Baggage()
    {
        Activity.Current = null;
        using var activity = new Activity("preserve").Start();
        activity.AddBaggage("CorrelationId", "from-baggage");

        var sink = new InMemorySink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .ApplyDotNetBuildingBlocksDefaults(new DotNetSerilogOptions
            {
                ApplicationName = "Tests.Serilog",
                UseConsole = false,
                IncludeMachineName = false,
                IncludeEnvironmentName = false,
                IncludeActivityEnricher = false,
                IncludeCorrelationEnricher = true
            })
            .CreateLogger();

        using (global::Serilog.Context.LogContext.PushProperty("CorrelationId", "from-context"))
        {
            logger.Information("Hello");
        }

        var logEvent = sink.Single();
        logEvent.Properties["CorrelationId"].Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("from-context");
    }
}
