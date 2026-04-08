using DotNetBuildingBlocks.Tracing.UnitTests.TestHelpers;

namespace DotNetBuildingBlocks.Tracing.UnitTests.Activities;

public sealed class ActivitySourceAccessorTests
{
    [Fact]
    public void Constructor_Should_Create_ActivitySource_With_Configured_Name_And_Version()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = "Tests.Tracing",
            ActivitySourceVersion = "1.2.3"
        });

        using var accessor = new ActivitySourceAccessor(options);

        accessor.ActivitySource.Name.Should().Be("Tests.Tracing");
        accessor.ActivitySource.Version.Should().Be("1.2.3");
    }

    [Fact]
    public void Constructor_Should_Throw_When_Options_Is_Null()
    {
        Action act = () => _ = new ActivitySourceAccessor(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public void Constructor_Should_Throw_When_ActivitySourceName_Is_Whitespace()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = " "
        });

        Action act = () => _ = new ActivitySourceAccessor(options);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("ActivitySourceName");
    }

    [Fact]
    public void StartActivity_Should_Return_Started_Activity_When_Listener_Is_Registered()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        var tags = new[]
        {
            new KeyValuePair<string, object?>("operation.name", "ImportCustomers"),
            new KeyValuePair<string, object?>("customer.count", 15)
        };

        using var activity = accessor.StartActivity("ImportCustomers", ActivityKind.Internal, tags: tags);

        activity.Should().NotBeNull();
        activity!.OperationName.Should().Be("ImportCustomers");
        activity.Kind.Should().Be(ActivityKind.Internal);
        activity.GetTagItem("operation.name").Should().Be("ImportCustomers");
        activity.GetTagItem("customer.count").Should().Be(15);
    }

    [Fact]
    public void StartActivity_Should_Return_Null_When_No_Listener_Is_Registered()
    {
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = "Tests.Tracing"
        }));

        using var activity = accessor.StartActivity("ImportCustomers");

        activity.Should().BeNull();
    }

    [Fact]
    public void StartActivity_Should_Use_Provided_ParentContext()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        var traceId = ActivityTraceId.CreateRandom();
        var spanId = ActivitySpanId.CreateRandom();
        var parentContext = new ActivityContext(
            traceId,
            spanId,
            ActivityTraceFlags.Recorded);

        using var activity = accessor.StartActivity(
            "ChildOperation",
            ActivityKind.Internal,
            parentContext);

        activity.Should().NotBeNull();
        activity!.TraceId.Should().Be(traceId);
        activity.ParentSpanId.Should().Be(spanId);
    }

    [Fact]
    public void StartActivity_Should_Include_Links_When_Provided()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        var linkedContext = new ActivityContext(
            ActivityTraceId.CreateRandom(),
            ActivitySpanId.CreateRandom(),
            ActivityTraceFlags.Recorded);

        var links = new[]
        {
            new ActivityLink(linkedContext)
        };

        using var activity = accessor.StartActivity(
            "ProcessBatch",
            ActivityKind.Internal,
            links: links);

        activity.Should().NotBeNull();
        activity!.Links.Should().ContainSingle();
        activity.Links.Single().Context.TraceId.Should().Be(linkedContext.TraceId);
    }

    [Fact]
    public void StartActivity_Should_Throw_When_Name_Is_Whitespace()
    {
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = "Tests.Tracing"
        }));

        Action act = () => accessor.StartActivity(" ");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("name");
    }

    [Fact]
    public void StartInternalActivity_Should_Start_Internal_Activity()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        using var activity = accessor.StartInternalActivity("InternalWork");

        activity.Should().NotBeNull();
        activity!.Kind.Should().Be(ActivityKind.Internal);
    }

    [Fact]
    public void StartClientActivity_Should_Start_Client_Activity()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        using var activity = accessor.StartClientActivity("CallDependency");

        activity.Should().NotBeNull();
        activity!.Kind.Should().Be(ActivityKind.Client);
    }

    [Fact]
    public void StartServerActivity_Should_Start_Server_Activity()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        using var activity = accessor.StartServerActivity("HandleRequest");

        activity.Should().NotBeNull();
        activity!.Kind.Should().Be(ActivityKind.Server);
    }

    [Fact]
    public void StartProducerActivity_Should_Start_Producer_Activity()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        using var activity = accessor.StartProducerActivity("PublishMessage");

        activity.Should().NotBeNull();
        activity!.Kind.Should().Be(ActivityKind.Producer);
    }

    [Fact]
    public void StartConsumerActivity_Should_Start_Consumer_Activity()
    {
        const string sourceName = "Tests.Tracing";

        using var listenerScope = new ActivityListenerScope(sourceName);
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = sourceName
        }));

        using var activity = accessor.StartConsumerActivity("ConsumeMessage");

        activity.Should().NotBeNull();
        activity!.Kind.Should().Be(ActivityKind.Consumer);
    }

    [Fact]
    public void Dispose_Should_Prevent_Further_Usage()
    {
        using var accessor = new ActivitySourceAccessor(Microsoft.Extensions.Options.Options.Create(new TracingOptions
        {
            ActivitySourceName = "Tests.Tracing"
        }));

        accessor.Dispose();

        Action act = () => accessor.StartActivity("ImportCustomers");

        act.Should().Throw<ObjectDisposedException>();
    }
}
