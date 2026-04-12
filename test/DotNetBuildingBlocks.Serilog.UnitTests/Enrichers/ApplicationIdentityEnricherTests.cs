namespace DotNetBuildingBlocks.Serilog.UnitTests.Enrichers;

public sealed class ApplicationIdentityEnricherTests
{
    [Fact]
    public void Enrich_Should_Add_ApplicationName_Property()
    {
        var enricher = new ApplicationIdentityEnricher("Tests.Serilog");
        var logEvent = TestEventFactory.CreateInformation();

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties.Should().ContainKey(ApplicationIdentityEnricher.ApplicationNamePropertyName);
        logEvent.Properties[ApplicationIdentityEnricher.ApplicationNamePropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("Tests.Serilog");
    }

    [Fact]
    public void Enrich_Should_Add_ApplicationVersion_When_Provided()
    {
        var enricher = new ApplicationIdentityEnricher("Tests.Serilog", "1.2.3");
        var logEvent = TestEventFactory.CreateInformation();

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties.Should().ContainKey(ApplicationIdentityEnricher.ApplicationVersionPropertyName);
        logEvent.Properties[ApplicationIdentityEnricher.ApplicationVersionPropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("1.2.3");
    }

    [Fact]
    public void Enrich_Should_Skip_ApplicationVersion_When_Null()
    {
        var enricher = new ApplicationIdentityEnricher("Tests.Serilog");
        var logEvent = TestEventFactory.CreateInformation();

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties.Should().NotContainKey(ApplicationIdentityEnricher.ApplicationVersionPropertyName);
    }

    [Fact]
    public void Enrich_Should_Preserve_Existing_Properties()
    {
        var enricher = new ApplicationIdentityEnricher("Tests.Serilog");
        var logEvent = TestEventFactory.CreateInformation();
        logEvent.AddPropertyIfAbsent(new LogEventProperty(
            ApplicationIdentityEnricher.ApplicationNamePropertyName,
            new ScalarValue("Existing")));

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties[ApplicationIdentityEnricher.ApplicationNamePropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be("Existing");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_ApplicationName_Is_Invalid(string? applicationName)
    {
        Action act = () => _ = new ApplicationIdentityEnricher(applicationName!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("applicationName");
    }
}
