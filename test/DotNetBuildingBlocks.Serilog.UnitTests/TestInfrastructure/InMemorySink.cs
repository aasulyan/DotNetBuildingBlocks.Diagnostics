using System.Collections.Concurrent;

namespace DotNetBuildingBlocks.Serilog.UnitTests.TestInfrastructure;

/// <summary>
/// Lightweight Serilog sink that captures emitted <see cref="LogEvent"/>s in memory for assertion.
/// </summary>
internal sealed class InMemorySink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEvent> events = new();

    public IReadOnlyCollection<LogEvent> Events => events.ToArray();

    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        events.Enqueue(logEvent);
    }

    public LogEvent Single() => Events.Should().ContainSingle().Which;
}
