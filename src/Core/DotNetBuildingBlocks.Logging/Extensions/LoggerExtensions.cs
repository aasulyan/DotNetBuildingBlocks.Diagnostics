using DotNetBuildingBlocks.Logging.Events;

namespace DotNetBuildingBlocks.Logging.Extensions;

/// <summary>
/// Provides provider-agnostic structured logging helpers.
/// </summary>
public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, object?, Exception?> OperationStartedMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Information,
            LoggingEventIds.OperationStarted,
            "Operation {OperationName} started for {Target}.");

    private static readonly Action<ILogger, string, object?, Exception?> OperationCompletedMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Information,
            LoggingEventIds.OperationCompleted,
            "Operation {OperationName} completed for {Target}.");

    private static readonly Action<ILogger, string, object?, Exception?> OperationFailedMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Error,
            LoggingEventIds.OperationFailed,
            "Operation {OperationName} failed for {Target}.");

    private static readonly Action<ILogger, object?, string, Exception?> ValidationFailedMessage =
        LoggerMessage.Define<object?, string>(
            LogLevel.Warning,
            LoggingEventIds.ValidationFailed,
            "Validation failed for {Target}. {Reason}");

    private static readonly Action<ILogger, string, object?, Exception?> UnhandledExceptionMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Error,
            LoggingEventIds.UnexpectedException,
            "Unhandled exception occurred during {OperationName} for {Target}.");

    private static readonly Action<ILogger, int, int, string, Exception?> RetryAttemptMessage =
        LoggerMessage.Define<int, int, string>(
            LogLevel.Warning,
            LoggingEventIds.RetryAttempt,
            "Retry attempt {AttemptNumber} of {MaxAttempts} for {OperationName}.");

    private static readonly Action<ILogger, string, object?, Exception?> DependencyFailureMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Error,
            LoggingEventIds.DependencyFailure,
            "External dependency {DependencyName} failed for {Target}.");

    private static readonly Action<ILogger, string, object?, Exception?> ResourceNotFoundMessage =
        LoggerMessage.Define<string, object?>(
            LogLevel.Information,
            LoggingEventIds.ResourceNotFound,
            "{ResourceType} was not found for {Target}.");

    /// <summary>
    /// Logs the start of an operation.
    /// </summary>
    public static void LogOperationStarted(this ILogger logger, string operationName, object? target = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ThrowIfNullOrWhiteSpace(operationName, nameof(operationName));

        OperationStartedMessage(logger, operationName, target, null);
    }

    /// <summary>
    /// Logs the successful completion of an operation.
    /// </summary>
    public static void LogOperationCompleted(this ILogger logger, string operationName, object? target = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ThrowIfNullOrWhiteSpace(operationName, nameof(operationName));

        OperationCompletedMessage(logger, operationName, target, null);
    }

    /// <summary>
    /// Logs a failed operation together with the thrown exception.
    /// </summary>
    public static void LogOperationFailed(this ILogger logger, Exception exception, string operationName, object? target = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(exception);
        ThrowIfNullOrWhiteSpace(operationName, nameof(operationName));

        OperationFailedMessage(logger, operationName, target, exception);
    }

    /// <summary>
    /// Logs a validation failure in a structured way.
    /// </summary>
    public static void LogValidationFailed(this ILogger logger, string reason, object? target = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ThrowIfNullOrWhiteSpace(reason, nameof(reason));

        ValidationFailedMessage(logger, target, reason, null);
    }

    /// <summary>
    /// Logs an unexpected exception in a structured way.
    /// </summary>
    public static void LogUnhandledException(this ILogger logger, Exception exception, string? operationName = null, object? target = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(exception);

        var resolvedOperationName = string.IsNullOrWhiteSpace(operationName) ? "UnknownOperation" : operationName;
        UnhandledExceptionMessage(logger, resolvedOperationName, target, exception);
    }

    /// <summary>
    /// Logs a retry attempt for an operation.
    /// </summary>
    public static void LogRetryAttempt(this ILogger logger, string operationName, int attemptNumber, int maxAttempts)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ThrowIfNullOrWhiteSpace(operationName, nameof(operationName));

        if (attemptNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber), "Attempt number must be greater than zero.");
        }

        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Max attempts must be greater than zero.");
        }

        if (attemptNumber > maxAttempts)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber), "Attempt number cannot be greater than max attempts.");
        }

        RetryAttemptMessage(logger, attemptNumber, maxAttempts, operationName, null);
    }

    /// <summary>
    /// Logs an external dependency failure.
    /// </summary>
    public static void LogDependencyFailure(this ILogger logger, Exception exception, string dependencyName, object? target = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(exception);
        ThrowIfNullOrWhiteSpace(dependencyName, nameof(dependencyName));

        DependencyFailureMessage(logger, dependencyName, target, exception);
    }

    /// <summary>
    /// Logs that a resource was not found.
    /// </summary>
    public static void LogResourceNotFound(this ILogger logger, string resourceType, object? target = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ThrowIfNullOrWhiteSpace(resourceType, nameof(resourceType));

        ResourceNotFoundMessage(logger, resourceType, target, null);
    }

    private static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
        }
    }
}
