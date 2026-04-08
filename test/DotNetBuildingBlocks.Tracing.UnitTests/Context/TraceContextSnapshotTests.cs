namespace DotNetBuildingBlocks.Tracing.UnitTests.Context;

public sealed class TraceContextSnapshotTests
{
    [Fact]
    public void CaptureCurrent_Should_Return_Null_When_No_Current_Activity()
    {
        var previous = Activity.Current;
        Activity.Current = null;

        try
        {
            var snapshot = TraceContextSnapshot.CaptureCurrent();

            snapshot.Should().BeNull();
        }
        finally
        {
            Activity.Current = previous;
        }
    }

    [Fact]
    public void CaptureCurrent_Should_Return_Snapshot_When_Current_Activity_Exists()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();
        activity.SetCorrelationId("corr-123");

        var snapshot = TraceContextSnapshot.CaptureCurrent();

        snapshot.Should().NotBeNull();
        snapshot!.TraceId.Should().Be(activity.TraceId.ToString());
        snapshot.SpanId.Should().Be(activity.SpanId.ToString());
        snapshot.TraceParent.Should().Be(activity.Id);
        snapshot.TraceState.Should().Be(activity.TraceStateString);
        snapshot.CorrelationId.Should().Be("corr-123");
    }

    [Fact]
    public void TryCaptureCurrent_Should_Return_False_When_No_Current_Activity()
    {
        var previous = Activity.Current;
        Activity.Current = null;

        try
        {
            var result = TraceContextSnapshot.TryCaptureCurrent(out var snapshot);

            result.Should().BeFalse();
            snapshot.Should().BeNull();
        }
        finally
        {
            Activity.Current = previous;
        }
    }

    [Fact]
    public void TryCaptureCurrent_Should_Return_True_When_Current_Activity_Exists()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        var result = TraceContextSnapshot.TryCaptureCurrent(out var snapshot);

        result.Should().BeTrue();
        snapshot.Should().NotBeNull();
        snapshot!.TraceId.Should().Be(activity.TraceId.ToString());
    }

    [Fact]
    public void ToHeaders_Should_Include_TraceParent_When_Available()
    {
        var snapshot = new TraceContextSnapshot(
            TraceId: ActivityTraceId.CreateRandom().ToString(),
            SpanId: ActivitySpanId.CreateRandom().ToString(),
            TraceParent: "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
            TraceState: null,
            CorrelationId: null);

        var headers = snapshot.ToHeaders();

        headers.Should().ContainKey("traceparent");
        headers["traceparent"].Should().Be("00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01");
    }

    [Fact]
    public void ToHeaders_Should_Include_TraceState_And_CorrelationId_When_Available()
    {
        var snapshot = new TraceContextSnapshot(
            TraceId: ActivityTraceId.CreateRandom().ToString(),
            SpanId: ActivitySpanId.CreateRandom().ToString(),
            TraceParent: "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
            TraceState: "rojo=00f067aa0ba902b7",
            CorrelationId: "corr-123");

        var headers = snapshot.ToHeaders();

        headers.Should().ContainKey("traceparent");
        headers.Should().ContainKey("tracestate");
        headers.Should().ContainKey(TracingTagNames.CorrelationId);

        headers["tracestate"].Should().Be("rojo=00f067aa0ba902b7");
        headers[TracingTagNames.CorrelationId].Should().Be("corr-123");
    }

    [Fact]
    public void FromActivity_Should_Create_Snapshot_From_Provided_Activity()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();
        activity.SetCorrelationId("corr-999");

        var snapshot = TraceContextSnapshot.FromActivity(activity);

        snapshot.TraceId.Should().Be(activity.TraceId.ToString());
        snapshot.SpanId.Should().Be(activity.SpanId.ToString());
        snapshot.TraceParent.Should().Be(activity.Id);
        snapshot.CorrelationId.Should().Be("corr-999");
    }

    [Fact]
    public void FromActivity_Should_Throw_When_Activity_Is_Null()
    {
        Action act = () => _ = TraceContextSnapshot.FromActivity(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("activity");
    }
}
