namespace DotNetBuildingBlocks.Tracing.UnitTests.Extensions;

public sealed class ActivityExtensionsTests
{
    [Fact]
    public void SetTagIfNotNull_Should_Set_Tag_When_Value_Is_Not_Null()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetTagIfNotNull("customer.id", 42);

        activity.GetTagItem("customer.id").Should().Be(42);
    }

    [Fact]
    public void SetTagIfNotNull_Should_Not_Set_Tag_When_Value_Is_Null()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetTagIfNotNull("customer.id", null);

        activity.GetTagItem("customer.id").Should().BeNull();
    }

    [Fact]
    public void SetTagIfNotNull_Should_Throw_When_Activity_Is_Null()
    {
        Activity? activity = null;

        Action act = () => activity!.SetTagIfNotNull("customer.id", 1);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("activity");
    }

    [Fact]
    public void SetTagIfNotNull_Should_Throw_When_Tag_Name_Is_Whitespace()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        Action act = () => activity.SetTagIfNotNull(" ", 1);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("name");
    }

    [Fact]
    public void SetCorrelationId_Should_Set_Correlation_Tag()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetCorrelationId("corr-123");

        activity.GetTagItem(TracingTagNames.CorrelationId).Should().Be("corr-123");
    }

    [Fact]
    public void SetTenantId_Should_Set_Tenant_Tag()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetTenantId("tenant-1");

        activity.GetTagItem(TracingTagNames.TenantId).Should().Be("tenant-1");
    }

    [Fact]
    public void SetUserId_Should_Set_User_Tag()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetUserId("user-1");

        activity.GetTagItem(TracingTagNames.UserId).Should().Be("user-1");
    }

    [Fact]
    public void SetOperationName_Should_Set_Operation_Tag()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetOperationName("ImportCustomers");

        activity.GetTagItem(TracingTagNames.OperationName).Should().Be("ImportCustomers");
    }

    [Fact]
    public void SetEntity_Should_Set_Entity_Type_And_Id()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetEntity("Order", 99);

        activity.GetTagItem(TracingTagNames.EntityType).Should().Be("Order");
        activity.GetTagItem(TracingTagNames.EntityId).Should().Be(99);
    }

    [Fact]
    public void SetEntity_Should_Set_Only_Entity_Type_When_EntityId_Is_Null()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetEntity("Order", null);

        activity.GetTagItem(TracingTagNames.EntityType).Should().Be("Order");
        activity.GetTagItem(TracingTagNames.EntityId).Should().BeNull();
    }

    [Fact]
    public void SetDependencySystem_Should_Set_Dependency_System_Tag()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetDependencySystem("sqlserver");

        activity.GetTagItem(TracingTagNames.DependencySystem).Should().Be("sqlserver");
    }

    [Fact]
    public void SetMessagingDestination_Should_Set_Messaging_Destination_Tag()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetMessagingDestination("orders-topic");

        activity.GetTagItem(TracingTagNames.MessagingDestination).Should().Be("orders-topic");
    }

    [Fact]
    public void MarkSuccess_Should_Set_Status_And_Outcome()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.MarkSuccess();

        activity.Status.Should().Be(ActivityStatusCode.Ok);
        activity.GetTagItem(TracingTagNames.Outcome).Should().Be("success");
    }

    [Fact]
    public void MarkError_With_Exception_Should_Set_Status_And_Error_Tags()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        var exception = new InvalidOperationException("Something failed.");

        activity.MarkError(exception);

        activity.Status.Should().Be(ActivityStatusCode.Error);
        activity.GetTagItem(TracingTagNames.Outcome).Should().Be("error");
        activity.GetTagItem(TracingTagNames.ErrorType).Should().Be(typeof(InvalidOperationException).FullName);
        activity.GetTagItem(TracingTagNames.ErrorMessage).Should().Be("Something failed.");
    }

    [Fact]
    public void MarkError_With_ErrorType_Should_Set_Status_And_Error_Tags()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.MarkError("validation", "Validation failed.");

        activity.Status.Should().Be(ActivityStatusCode.Error);
        activity.GetTagItem(TracingTagNames.Outcome).Should().Be("error");
        activity.GetTagItem(TracingTagNames.ErrorType).Should().Be("validation");
        activity.GetTagItem(TracingTagNames.ErrorMessage).Should().Be("Validation failed.");
    }

    [Fact]
    public void SetOutcome_Should_Set_Custom_Outcome_Tag()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        activity.SetOutcome("partial-success");

        activity.GetTagItem(TracingTagNames.Outcome).Should().Be("partial-success");
    }

    [Fact]
    public void SetCorrelationId_Should_Return_Same_Activity()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        var returned = activity.SetCorrelationId("corr-123");

        returned.Should().BeSameAs(activity);
    }
}
