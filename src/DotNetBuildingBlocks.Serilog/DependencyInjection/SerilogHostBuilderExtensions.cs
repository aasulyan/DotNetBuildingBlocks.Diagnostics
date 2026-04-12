using DotNetBuildingBlocks.Serilog.Configuration;
using DotNetBuildingBlocks.Serilog.Internal;
using DotNetBuildingBlocks.Serilog.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Settings.Configuration;

namespace DotNetBuildingBlocks.Serilog.DependencyInjection;

/// <summary>
/// <see cref="IHostBuilder"/> extensions that wire Serilog into the .NET generic host using
/// the <c>DotNetBuildingBlocks</c> defaults.
/// </summary>
public static class SerilogHostBuilderExtensions
{
    /// <summary>
    /// Wires Serilog into the host pipeline using package-owned defaults.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="configure">An optional delegate used to mutate the <see cref="DotNetSerilogOptions"/>.</param>
    /// <returns>The same <see cref="IHostBuilder"/> instance.</returns>
    /// <remarks>
    /// <para>
    /// Configuration precedence: when <see cref="DotNetSerilogOptions.ReadFromConfiguration"/> is enabled,
    /// the configured Serilog section is read first and the package defaults are applied on top, so
    /// explicit code-based options take precedence over configuration values.
    /// </para>
    /// <para>
    /// This extension also accepts the <c>ConfigureHostBuilder</c> exposed by
    /// <c>WebApplicationBuilder.Host</c> because it implements <see cref="IHostBuilder"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hostBuilder"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when the resulting <see cref="DotNetSerilogOptions"/> are invalid.</exception>
    public static IHostBuilder UseDotNetBuildingBlocksSerilog(
        this IHostBuilder hostBuilder,
        Action<DotNetSerilogOptions>? configure = null)
    {
        ArgumentGuard.ThrowIfNull(hostBuilder, nameof(hostBuilder));

        var options = new DotNetSerilogOptions();
        configure?.Invoke(options);
        SerilogOptionsValidator.Validate(options);

        return hostBuilder.UseSerilog((context, _, loggerConfiguration) =>
        {
            ApplyConfigurationAndDefaults(loggerConfiguration, options, context.Configuration, context.HostingEnvironment.EnvironmentName);
        });
    }

    internal static void ApplyConfigurationAndDefaults(
        LoggerConfiguration loggerConfiguration,
        DotNetSerilogOptions options,
        IConfiguration? configuration,
        string? environmentName)
    {
        if (options.ReadFromConfiguration && configuration is not null)
        {
            loggerConfiguration.ReadFrom.Configuration(
                configuration,
                new ConfigurationReaderOptions { SectionName = options.ConfigurationSectionName });
        }

        loggerConfiguration.ApplyDotNetBuildingBlocksDefaults(options, environmentName);
    }
}
