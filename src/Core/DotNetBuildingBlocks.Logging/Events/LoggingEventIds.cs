namespace DotNetBuildingBlocks.Logging.Events
{
    /// <summary>
    /// Common reusable event identifiers used by this package.
    /// </summary>
    public static class LoggingEventIds
    {
        public static readonly EventId OperationStarted = new(1000, nameof(OperationStarted));
        public static readonly EventId OperationCompleted = new(1001, nameof(OperationCompleted));
        public static readonly EventId OperationFailed = new(1002, nameof(OperationFailed));

        public static readonly EventId ValidationFailed = new(1100, nameof(ValidationFailed));

        public static readonly EventId RetryAttempt = new(1200, nameof(RetryAttempt));
        public static readonly EventId DependencyFailure = new(1300, nameof(DependencyFailure));
        public static readonly EventId ResourceNotFound = new(1400, nameof(ResourceNotFound));

        public static readonly EventId UnexpectedException = new(1500, nameof(UnexpectedException));
    }
}
