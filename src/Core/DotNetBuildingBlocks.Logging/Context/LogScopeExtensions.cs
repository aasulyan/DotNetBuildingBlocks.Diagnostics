using DotNetBuildingBlocks.Logging.Internal;

namespace DotNetBuildingBlocks.Logging.Context
{
    /// <summary>
    /// Provides helpers for starting structured logging scopes.
    /// </summary>
    public static class LogScopeExtensions
    {
        /// <summary>
        /// Begins a scope from the provided name/value tuples.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="properties">The structured properties to include in the scope.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
        public static IDisposable BeginPropertyScope(this ILogger logger, params (string Name, object? Value)[] properties)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(properties);

            KeyValuePair<string, object?>[] keyValuePairs = new KeyValuePair<string, object?>[properties.Length];
            for (int index = 0; index < properties.Length; index++)
            {
                keyValuePairs[index] = new KeyValuePair<string, object?>(properties[index].Name, properties[index].Value);
            }

            return logger.BeginPropertyScope(keyValuePairs);
        }

        /// <summary>
        /// Begins a scope from the provided structured properties.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="properties">The structured properties to include in the scope.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
        public static IDisposable BeginPropertyScope(this ILogger logger, IEnumerable<KeyValuePair<string, object?>> properties)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(properties);

            LogScopeState state = new(properties);
            return logger.BeginScope(state) ?? NullScope.Instance;
        }

        /// <summary>
        /// Begins a scope with a correlation identifier.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
        public static IDisposable BeginCorrelationScope(this ILogger logger, string correlationId)
        {
            ArgumentNullException.ThrowIfNull(logger);

            return string.IsNullOrWhiteSpace(correlationId)
                ? throw new ArgumentException("Correlation ID cannot be null, empty, or whitespace.", nameof(correlationId))
                : logger.BeginPropertyScope((KnownLogPropertyNames.CorrelationId, correlationId));
        }

        /// <summary>
        /// Begins a scope with operation metadata and optional correlation identifier.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="operationName">The operation name.</param>
        /// <param name="correlationId">The optional correlation identifier.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
        public static IDisposable BeginOperationScope(this ILogger logger, string operationName, string? correlationId = null)
        {
            ArgumentNullException.ThrowIfNull(logger);

            return string.IsNullOrWhiteSpace(operationName)
                ? throw new ArgumentException("Operation name cannot be null, empty, or whitespace.", nameof(operationName))
                : correlationId is null
                ? logger.BeginPropertyScope((KnownLogPropertyNames.OperationName, operationName))
                : string.IsNullOrWhiteSpace(correlationId)
                ? throw new ArgumentException("Correlation ID cannot be empty or whitespace when provided.", nameof(correlationId))
                : logger.BeginPropertyScope(
                (KnownLogPropertyNames.OperationName, operationName),
                (KnownLogPropertyNames.CorrelationId, correlationId));
        }

        /// <summary>
        /// Begins a scope with entity metadata and optional operation name.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="operationName">The optional operation name.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
        public static IDisposable BeginEntityScope(this ILogger logger, string entityType, object? entityId, string? operationName = null)
        {
            ArgumentNullException.ThrowIfNull(logger);

            if (string.IsNullOrWhiteSpace(entityType))
            {
                throw new ArgumentException("Entity type cannot be null, empty, or whitespace.", nameof(entityType));
            }

            if (operationName is not null && string.IsNullOrWhiteSpace(operationName))
            {
                throw new ArgumentException("Operation name cannot be empty or whitespace when provided.", nameof(operationName));
            }

            KeyValuePair<string, object?>[] properties = operationName is null
                ?
                [
                    new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityType, entityType),
                    new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityId, entityId)
                ]
                :
                [
                    new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityType, entityType),
                    new KeyValuePair<string, object?>(KnownLogPropertyNames.EntityId, entityId),
                    new KeyValuePair<string, object?>(KnownLogPropertyNames.OperationName, operationName)
                ];

            return logger.BeginPropertyScope(properties);
        }
    }
}
