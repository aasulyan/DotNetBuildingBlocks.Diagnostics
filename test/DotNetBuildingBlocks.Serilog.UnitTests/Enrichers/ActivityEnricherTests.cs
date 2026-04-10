namespace DotNetBuildingBlocks.Serilog.UnitTests.Enrichers;

public sealed class ActivityEnricherTests
{
    public ActivityEnricherTests()
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
    }

    [Fact]
    public void Enrich_Should_Add_Trace_And_Span_Properties_When_Activity_Is_Active()
    {
        using var activity = new Activity("test-op").Start();

        var enricher = new ActivityEnricher();
        var logEvent = TestEventFactory.CreateInformation();

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties.Should().ContainKey(ActivityEnricher.TraceIdPropertyName);
        logEvent.Properties.Should().ContainKey(ActivityEnricher.SpanIdPropertyName);

        logEvent.Properties[ActivityEnricher.TraceIdPropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be(activity.TraceId.ToString());

        logEvent.Properties[ActivityEnricher.SpanIdPropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be(activity.SpanId.ToString());
    }

    [Fact]
    public void Enrich_Should_Add_ParentSpanId_When_Parent_Activity_Exists()
    {
        using var parent = new Activity("parent-op").Start();
        using var child = new Activity("child-op").Start();

        var enricher = new ActivityEnricher();
        var logEvent = TestEventFactory.CreateInformation();

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties.Should().ContainKey(ActivityEnricher.ParentSpanIdPropertyName);
        logEvent.Properties[ActivityEnricher.ParentSpanIdPropertyName]
            .Should().BeOfType<ScalarValue>()
            .Which.Value.Should().Be(parent.SpanId.ToString());
    }

    [Fact]
    public void Enrich_Should_Add_Nothing_When_No_Current_Activity()
    {
        Activity.Current = null;

        var enricher = new ActivityEnricher();
        var logEvent = TestEventFactory.CreateInformation();

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties.Should().NotContainKey(ActivityEnricher.TraceIdPropertyName);
        logEvent.Properties.Should().NotContainKey(ActivityEnricher.SpanIdPropertyName);
        logEvent.Properties.Should().NotContainKey(ActivityEnricher.ParentSpanIdPropertyName);
    }

    [Fact]
    public void Enrich_Should_Skip_ParentSpanId_When_No_Parent()
    {
        Activity.Current = null;
        using var activity = new Activity("root-op").Start();

        var enricher = new ActivityEnricher();
        var logEvent = TestEventFactory.CreateInformation();

        enricher.Enrich(logEvent, new TestPropertyFactory());

        logEvent.Properties.Should().NotContainKey(ActivityEnricher.ParentSpanIdPropertyName);
    }
}
