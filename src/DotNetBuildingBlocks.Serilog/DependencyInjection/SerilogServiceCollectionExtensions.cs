using DotNetBuildingBlocks.Serilog.Internal;
using DotNetBuildingBlocks.Serilog.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetBuildingBlocks.Serilog.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions that wire Serilog into a service collection
/// using the <c>DotNetBuildingBlocks</c> defaults.
/// </summary>
/// <remarks>
/// Prefer <see cref="SerilogHostBuilderExtensions.UseDotNetBuildingBlocksSerilog"/> when working with
/// the .NET generic host because Serilog is normally configured at host level. This service collection
/// entry point is provided for scenarios where only an <see cref="IServiceCollection"/> is available
/// (for example, custom composition roots and tests).
/// </remarks>
public static class SerilogServiceCollectionExtensions
{
    /// <summary>
    /// Registers Serilog into the supplied service collection using the package-owned defaults.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An optional delegate used to mutate the <see cref="DotNetSerilogOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when the resulting <see cref="DotNetSerilogOptions"/> are invalid.</exception>
    public static IServiceCollection AddDotNetBuildingBlocksSerilog(
        this IServiceCollection services,
        Action<DotNetSerilogOptions>? configure = null)
    {
        ArgumentGuard.ThrowIfNull(services, nameof(services));

        var options = new DotNetSerilogOptions();
        configure?.Invoke(options);
        SerilogOptionsValidator.Validate(options);

        // Ensure Microsoft.Extensions.Logging primitives (open-generic ILogger<>, ILoggerFactory)
        // are registered. AddLogging is idempotent because it uses TryAdd internally.
        services.AddLogging();

        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var environment = serviceProvider.GetService<IHostEnvironment>();

            SerilogHostBuilderExtensions.ApplyConfigurationAndDefaults(
                loggerConfiguration,
                options,
                configuration,
                environment?.EnvironmentName);
        });

        return services;
    }
}
