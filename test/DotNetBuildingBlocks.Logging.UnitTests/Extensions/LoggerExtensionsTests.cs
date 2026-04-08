using DotNetBuildingBlocks.Logging.Events;
using DotNetBuildingBlocks.Logging.Extensions;
using DotNetBuildingBlocks.Logging.UnitTests.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNetBuildingBlocks.Logging.UnitTests.Extensions;

public sealed class LoggerExtensionsTests
{
    [Fact]
    public void LogOperationStarted_Should_Write_Information_Log()
    {
        var logger = new TestLogger();

        logger.LogOperationStarted("ProcessOrder", 123);

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Information);
        entry.EventId.Should().Be(LoggingEventIds.OperationStarted);
        entry.Message.Should().Contain("ProcessOrder").And.Contain("123");
        entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "ProcessOrder"));
        entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, 123));
    }

    [Fact]
    public void LogOperationCompleted_Should_Write_Information_Log()
    {
        var logger = new TestLogger();

        logger.LogOperationCompleted("ProcessOrder", 123);

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Information);
        entry.EventId.Should().Be(LoggingEventIds.OperationCompleted);
    }

    [Fact]
    public void LogOperationFailed_Should_Write_Error_Log_With_Exception()
    {
        var logger = new TestLogger();
        var exception = new InvalidOperationException("boom");

        logger.LogOperationFailed(exception, "ProcessOrder", 123);

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Error);
        entry.EventId.Should().Be(LoggingEventIds.OperationFailed);
        entry.Exception.Should().BeSameAs(exception);
        entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "ProcessOrder"));
        entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, 123));
    }

    [Fact]
    public void LogValidationFailed_Should_Write_Warning_Log()
    {
        var logger = new TestLogger();

        logger.LogValidationFailed("Input validation failed.", "command-1");

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Warning);
        entry.EventId.Should().Be(LoggingEventIds.ValidationFailed);
        entry.State.Should().Contain(x => x.Key == "Reason" && Equals(x.Value, "Input validation failed."));
        entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, "command-1"));
    }

    [Fact]
    public void LogUnhandledException_Should_Write_Error_Log_With_Default_Operation_Name_When_Missing()
    {
        var logger = new TestLogger();
        var exception = new Exception("unexpected");

        logger.LogUnhandledException(exception);

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Error);
        entry.EventId.Should().Be(LoggingEventIds.UnexpectedException);
        entry.Exception.Should().BeSameAs(exception);
        entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "UnknownOperation"));
    }

    [Fact]
    public void LogRetryAttempt_Should_Write_Warning_Log()
    {
        var logger = new TestLogger();

        logger.LogRetryAttempt("ImportCustomers", 2, 5);

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Warning);
        entry.EventId.Should().Be(LoggingEventIds.RetryAttempt);
        entry.State.Should().Contain(x => x.Key == "AttemptNumber" && Equals(x.Value, 2));
        entry.State.Should().Contain(x => x.Key == "MaxAttempts" && Equals(x.Value, 5));
        entry.State.Should().Contain(x => x.Key == "OperationName" && Equals(x.Value, "ImportCustomers"));
    }

    [Fact]
    public void LogDependencyFailure_Should_Write_Error_Log()
    {
        var logger = new TestLogger();
        var exception = new TimeoutException("timeout");

        logger.LogDependencyFailure(exception, "PaymentsApi", "order-1");

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Error);
        entry.EventId.Should().Be(LoggingEventIds.DependencyFailure);
        entry.State.Should().Contain(x => x.Key == "DependencyName" && Equals(x.Value, "PaymentsApi"));
        entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, "order-1"));
    }

    [Fact]
    public void LogResourceNotFound_Should_Write_Information_Log()
    {
        var logger = new TestLogger();

        logger.LogResourceNotFound("Order", 404);

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Information);
        entry.EventId.Should().Be(LoggingEventIds.ResourceNotFound);
        entry.State.Should().Contain(x => x.Key == "ResourceType" && Equals(x.Value, "Order"));
        entry.State.Should().Contain(x => x.Key == "Target" && Equals(x.Value, 404));
    }
}
