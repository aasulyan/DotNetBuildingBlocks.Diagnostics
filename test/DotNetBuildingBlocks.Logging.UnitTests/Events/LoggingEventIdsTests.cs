using DotNetBuildingBlocks.Logging.Events;
using FluentAssertions;
using Xunit;

namespace DotNetBuildingBlocks.Logging.UnitTests.Events;

public sealed class LoggingEventIdsTests
{
    [Fact]
    public void EventIds_Should_Be_Unique()
    {
        var values = new[]
        {
            LoggingEventIds.OperationStarted,
            LoggingEventIds.OperationCompleted,
            LoggingEventIds.OperationFailed,
            LoggingEventIds.ValidationFailed,
            LoggingEventIds.RetryAttempt,
            LoggingEventIds.DependencyFailure,
            LoggingEventIds.ResourceNotFound,
            LoggingEventIds.UnexpectedException
        };

        values.Select(x => x.Id).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void EventNames_Should_Be_Stable()
    {
        LoggingEventIds.OperationStarted.Name.Should().Be(nameof(LoggingEventIds.OperationStarted));
        LoggingEventIds.ValidationFailed.Name.Should().Be(nameof(LoggingEventIds.ValidationFailed));
        LoggingEventIds.UnexpectedException.Name.Should().Be(nameof(LoggingEventIds.UnexpectedException));
    }
}
