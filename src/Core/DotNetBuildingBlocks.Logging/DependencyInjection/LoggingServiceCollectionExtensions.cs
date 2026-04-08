using DotNetBuildingBlocks.Logging.Context;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetBuildingBlocks.Logging.DependencyInjection
{
    /// <summary>
    /// Provides dependency injection helpers for <c>DotNetBuildingBlocks.Logging</c>.
    /// </summary>
    public static class LoggingServiceCollectionExtensions
    {
        /// <summary>
        /// Registers package-owned logging services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection instance.</returns>
        public static IServiceCollection AddDotNetBuildingBlocksLogging(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddSingleton<ILogScopeStateFactory, DefaultLogScopeStateFactory>();
            return services;
        }
    }
}
