using DotNetBuildingBlocks.Serilog.Enrichers;
using DotNetBuildingBlocks.Serilog.Internal;
using DotNetBuildingBlocks.Serilog.Options;
using Serilog.Context;

namespace DotNetBuildingBlocks.Serilog.Configuration;

/// <summary>
/// Extension methods that apply <c>DotNetBuildingBlocks</c> defaults to a Serilog
/// <see cref="LoggerConfiguration"/> instance.
/// </summary>
public static class LoggerConfigurationExtensions
{
    /// <summary>
    /// Applies the package-owned defaults to the supplied <see cref="LoggerConfiguration"/>.
    /// </summary>
    /// <param name="loggerConfiguration">The Serilog logger configuration to mutate.</param>
    /// <param name="options">The package options describing how Serilog should be configured.</param>
    /// <param name="environmentName">
    /// The optional host environment name. When not <c>null</c>, it is added as the
    /// <c>EnvironmentName</c> property if <see cref="DotNetSerilogOptions.IncludeEnvironmentName"/>
    /// is enabled.
    /// </param>
    /// <returns>The same <see cref="LoggerConfiguration"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="loggerConfiguration"/> or <paramref name="options"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="options"/> contains invalid values.</exception>
    public static LoggerConfiguration ApplyDotNetBuildingBlocksDefaults(
        this LoggerConfiguration loggerConfiguration,
        DotNetSerilogOptions options,
        string? environmentName = null)
    {
        ArgumentGuard.ThrowIfNull(loggerConfiguration, nameof(loggerConfiguration));
        SerilogOptionsValidator.Validate(options);

        loggerConfiguration.MinimumLevel.Is(options.MinimumLevel);

        loggerConfiguration.Enrich.FromLogContext();
        loggerConfiguration.Enrich.With(new ApplicationIdentityEnricher(
            options.ApplicationName,
            options.ApplicationVersion));

        if (options.IncludeActivityEnricher)
        {
            loggerConfiguration.Enrich.With<ActivityEnricher>();
        }

        if (options.IncludeCorrelationEnricher)
        {
            loggerConfiguration.Enrich.With<CorrelationEnricher>();
        }

        if (options.IncludeMachineName)
        {
            loggerConfiguration.Enrich.WithProperty("MachineName", Environment.MachineName);
        }

        var resolvedEnvironment = ResolveEnvironmentName(environmentName);
        if (options.IncludeEnvironmentName && !string.IsNullOrWhiteSpace(resolvedEnvironment))
        {
            loggerConfiguration.Enrich.WithProperty("EnvironmentName", resolvedEnvironment);
        }

        if (options.IncludeThreadId)
        {
            loggerConfiguration.Enrich.With<ThreadIdEnricher>();
        }

        if (options.UseConsole)
        {
            if (options.UseJsonConsoleFormatter)
            {
                loggerConfiguration.WriteTo.Console(new global::Serilog.Formatting.Json.JsonFormatter());
            }
            else
            {
                loggerConfiguration.WriteTo.Console();
            }
        }

        if (options.UseDebug)
        {
            loggerConfiguration.WriteTo.Debug();
        }

        return loggerConfiguration;
    }

    private static string? ResolveEnvironmentName(string? environmentName)
    {
        if (!string.IsNullOrWhiteSpace(environmentName))
        {
            return environmentName;
        }

        var dotnetEnvironment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (!string.IsNullOrWhiteSpace(dotnetEnvironment))
        {
            return dotnetEnvironment;
        }

        var aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.IsNullOrWhiteSpace(aspnetCoreEnvironment) ? null : aspnetCoreEnvironment;
    }

    /// <summary>
    /// Internal enricher that adds the current managed thread identifier as the <c>ThreadId</c> property.
    /// </summary>
    private sealed class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ThreadId",
                Environment.CurrentManagedThreadId));
        }
    }

    /// <summary>
    /// Internal enricher that promotes a <c>CorrelationId</c> from <see cref="Activity.Current"/> baggage
    /// onto a log event when no <c>CorrelationId</c> is already present (for example, from
    /// <see cref="LogContext"/> or a Microsoft logger scope).
    /// </summary>
    private sealed class CorrelationEnricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";
        private const string CorrelationIdBaggageName = "correlation.id";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.ContainsKey(CorrelationIdPropertyName))
            {
                return;
            }

            var activity = Activity.Current;
            if (activity is null)
            {
                return;
            }

            var correlationId = activity.GetBaggageItem(CorrelationIdPropertyName)
                ?? activity.GetBaggageItem(CorrelationIdBaggageName);

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                return;
            }

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                CorrelationIdPropertyName,
                correlationId));
        }
    }
}
