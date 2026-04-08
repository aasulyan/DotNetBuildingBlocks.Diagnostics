namespace DotNetBuildingBlocks.Tracing.UnitTests.Tags;

public sealed class TracingTagNamesTests
{
    [Fact]
    public void Tag_Names_Should_Match_Expected_Values()
    {
        TracingTagNames.CorrelationId.Should().Be("correlation.id");
        TracingTagNames.TenantId.Should().Be("tenant.id");
        TracingTagNames.UserId.Should().Be("user.id");
        TracingTagNames.OperationName.Should().Be("operation.name");
        TracingTagNames.EntityType.Should().Be("entity.type");
        TracingTagNames.EntityId.Should().Be("entity.id");
        TracingTagNames.Outcome.Should().Be("outcome");
        TracingTagNames.ErrorType.Should().Be("error.type");
        TracingTagNames.ErrorMessage.Should().Be("error.message");
        TracingTagNames.DependencySystem.Should().Be("dependency.system");
        TracingTagNames.MessagingDestination.Should().Be("messaging.destination");
    }
}
