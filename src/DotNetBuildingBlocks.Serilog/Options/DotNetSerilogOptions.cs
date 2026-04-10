namespace DotNetBuildingBlocks.Serilog.Options;

/// <summary>
/// Configures the Serilog integration provided by <c>DotNetBuildingBlocks.Serilog</c>.
/// </summary>
public sealed class DotNetSerilogOptions
{
    /// <summary>
    /// Gets or sets the application name attached to every log event as the <c>ApplicationName</c> property.
    /// </summary>
    /// <remarks>
    /// This value is required. Registration helpers will throw if it is null, empty, or whitespace.
    /// </remarks>
    public string ApplicationName { get; set; } = "DotNetBuildingBlocks";

    /// <summary>
    /// Gets or sets the optional application version attached to every log event as the <c>ApplicationVersion</c> property.
    /// </summary>
    public string? ApplicationVersion { get; set; }

    /// <summary>
    /// Gets or sets the minimum log event level applied to the Serilog pipeline.
    /// </summary>
    public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Information;

    /// <summary>
    /// Gets or sets a value indicating whether the console sink should be enabled.
    /// </summary>
    public bool UseConsole { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the debug sink should be enabled.
    /// </summary>
    public bool UseDebug { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the console sink should emit JSON-formatted output.
    /// </summary>
    /// <remarks>
    /// Only effective when <see cref="UseConsole"/> is <c>true</c>.
    /// </remarks>
    public bool UseJsonConsoleFormatter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>MachineName</c> property should be added to every log event.
    /// </summary>
    public bool IncludeMachineName { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the <c>EnvironmentName</c> property should be added to every log event.
    /// </summary>
    /// <remarks>
    /// When the host environment is not provided, the value of the <c>DOTNET_ENVIRONMENT</c>
    /// (or <c>ASPNETCORE_ENVIRONMENT</c>) environment variable is used as a fallback.
    /// </remarks>
    public bool IncludeEnvironmentName { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a <c>ThreadId</c> property should be added to every log event.
    /// </summary>
    public bool IncludeThreadId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the activity-aware enricher should add
    /// <c>TraceId</c>, <c>SpanId</c>, and <c>ParentSpanId</c> properties from <see cref="Activity.Current"/>.
    /// </summary>
    public bool IncludeActivityEnricher { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether <c>CorrelationId</c> should be promoted from
    /// <see cref="Activity.Current"/> baggage onto each log event when no value is already present.
    /// </summary>
    /// <remarks>
    /// When the consumer already pushes a <c>CorrelationId</c> into <see cref="global::Serilog.Context.LogContext"/>
    /// or via a Microsoft logger scope, that value is preserved and not overwritten.
    /// </remarks>
    public bool IncludeCorrelationEnricher { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the Serilog pipeline should also read from
    /// <see cref="Microsoft.Extensions.Configuration.IConfiguration"/> via <c>Serilog.Settings.Configuration</c>.
    /// </summary>
    /// <remarks>
    /// When enabled, configuration is read first and then code-based options are applied,
    /// so explicit code values take precedence over configuration values.
    /// </remarks>
    public bool ReadFromConfiguration { get; set; } = true;

    /// <summary>
    /// Gets or sets the configuration section name passed to <c>Serilog.Settings.Configuration</c>.
    /// </summary>
    public string? ConfigurationSectionName { get; set; } = "Serilog";
}
