using Serilog.Parsing;

namespace DotNetBuildingBlocks.Serilog.UnitTests.TestInfrastructure;

internal static class TestEventFactory
{
    public static LogEvent CreateInformation(string template = "Hello {Subject}", params object[] arguments)
    {
        var parser = new MessageTemplateParser();
        var messageTemplate = parser.Parse(template);

        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            exception: null,
            messageTemplate,
            properties: Array.Empty<LogEventProperty>());
    }
}

internal sealed class TestPropertyFactory : ILogEventPropertyFactory
{
    public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false)
    {
        return new LogEventProperty(name, value is null ? new ScalarValue(null) : new ScalarValue(value));
    }
}
