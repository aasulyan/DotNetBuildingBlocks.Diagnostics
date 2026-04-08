using DotNetBuildingBlocks.Logging.Context;
using DotNetBuildingBlocks.Logging.UnitTests.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNetBuildingBlocks.Logging.UnitTests.Context;

public sealed class LogScopeExtensionsTests
{
    [Fact]
    public void BeginPropertyScope_FromTuples_Should_Create_Structured_State()
    {
        var logger = new TestLogger();

        using var _ = logger.BeginPropertyScope(
            ("CorrelationId", "corr-123"),
            ("TenantId", "tenant-1"));

        logger.Scopes.Should().HaveCount(1);

        var state = logger.Scopes[0].State.Should().BeOfType<LogScopeState>().Subject;
        state.Should().Contain(x => x.Key == "CorrelationId" && Equals(x.Value, "corr-123"));
        state.Should().Contain(x => x.Key == "TenantId" && Equals(x.Value, "tenant-1"));
    }

    [Fact]
    public void BeginPropertyScope_FromEnumerable_Should_Create_Structured_State()
    {
        var logger = new TestLogger();

        using var _ = logger.BeginPropertyScope(
        [
            new KeyValuePair<string, object?>("UserId", "user-1"),
            new KeyValuePair<string, object?>("Feature", "Checkout")
        ]);

        var state = logger.Scopes[0].State.Should().BeOfType<LogScopeState>().Subject;
        state.Should().Contain(x => x.Key == "UserId" && Equals(x.Value, "user-1"));
        state.Should().Contain(x => x.Key == "Feature" && Equals(x.Value, "Checkout"));
    }

    [Fact]
    public void BeginCorrelationScope_Should_Include_CorrelationId()
    {
        var logger = new TestLogger();

        using var _ = logger.BeginCorrelationScope("corr-123");

        var state = logger.Scopes[0].State.Should().BeOfType<LogScopeState>().Subject;
        state.Should().ContainSingle(x => x.Key == KnownLogPropertyNames.CorrelationId && Equals(x.Value, "corr-123"));
    }

    [Fact]
    public void BeginOperationScope_Should_Include_OperationName_And_CorrelationId()
    {
        var logger = new TestLogger();

        using var _ = logger.BeginOperationScope("ProcessOrder", "corr-123");

        var state = logger.Scopes[0].State.Should().BeOfType<LogScopeState>().Subject;
        state.Should().Contain(x => x.Key == KnownLogPropertyNames.OperationName && Equals(x.Value, "ProcessOrder"));
        state.Should().Contain(x => x.Key == KnownLogPropertyNames.CorrelationId && Equals(x.Value, "corr-123"));
    }

    [Fact]
    public void BeginEntityScope_Should_Include_EntityType_And_EntityId_And_OperationName()
    {
        var logger = new TestLogger();

        using var _ = logger.BeginEntityScope("Order", 42, "CreateOrder");

        var state = logger.Scopes[0].State.Should().BeOfType<LogScopeState>().Subject;
        state.Should().Contain(x => x.Key == KnownLogPropertyNames.EntityType && Equals(x.Value, "Order"));
        state.Should().Contain(x => x.Key == KnownLogPropertyNames.EntityId && Equals(x.Value, 42));
        state.Should().Contain(x => x.Key == KnownLogPropertyNames.OperationName && Equals(x.Value, "CreateOrder"));
    }

    [Fact]
    public void BeginPropertyScope_Should_Throw_When_Logger_Is_Null()
    {
        ILogger logger = null!;

        var action = () => logger.BeginPropertyScope(("CorrelationId", "corr-1"));

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void BeginCorrelationScope_Should_Throw_When_CorrelationId_Is_Invalid()
    {
        var logger = new TestLogger();

        var action = () => logger.BeginCorrelationScope(" ");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BeginOperationScope_Should_Throw_When_OperationName_Is_Invalid()
    {
        var logger = new TestLogger();

        var action = () => logger.BeginOperationScope(" ");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BeginEntityScope_Should_Throw_When_EntityType_Is_Invalid()
    {
        var logger = new TestLogger();

        var action = () => logger.BeginEntityScope(" ", 1);

        action.Should().Throw<ArgumentException>();
    }
}
