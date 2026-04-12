using DotNetBuildingBlocks.Serilog.Internal;

namespace DotNetBuildingBlocks.Serilog.Enrichers;

/// <summary>
/// Enriches log events with stable application identity properties.
/// </summary>
/// <remarks>
/// Adds <c>ApplicationName</c> always and <c>ApplicationVersion</c> when supplied.
/// Existing properties of the same name are preserved.
/// </remarks>
public sealed class ApplicationIdentityEnricher : ILogEventEnricher
{
    /// <summary>
    /// The well-known property name for the application name.
    /// </summary>
    public const string ApplicationNamePropertyName = "ApplicationName";

    /// <summary>
    /// The well-known property name for the application version.
    /// </summary>
    public const string ApplicationVersionPropertyName = "ApplicationVersion";

    private readonly LogEventProperty applicationNameProperty;
    private readonly LogEventProperty? applicationVersionProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationIdentityEnricher"/> class.
    /// </summary>
    /// <param name="applicationName">The application name. Required.</param>
    /// <param name="applicationVersion">The optional application version.</param>
    public ApplicationIdentityEnricher(string applicationName, string? applicationVersion = null)
    {
        ArgumentGuard.ThrowIfNullOrWhiteSpace(applicationName, nameof(applicationName));

        applicationNameProperty = new LogEventProperty(
            ApplicationNamePropertyName,
            new ScalarValue(applicationName));

        if (!string.IsNullOrWhiteSpace(applicationVersion))
        {
            applicationVersionProperty = new LogEventProperty(
                ApplicationVersionPropertyName,
                new ScalarValue(applicationVersion));
        }
    }

    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentGuard.ThrowIfNull(logEvent, nameof(logEvent));

        logEvent.AddPropertyIfAbsent(applicationNameProperty);

        if (applicationVersionProperty is not null)
        {
            logEvent.AddPropertyIfAbsent(applicationVersionProperty);
        }
    }
}
