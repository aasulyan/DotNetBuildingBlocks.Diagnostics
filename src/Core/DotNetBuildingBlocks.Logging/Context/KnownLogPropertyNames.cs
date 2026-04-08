namespace DotNetBuildingBlocks.Logging.Context;

/// <summary>
/// Provides well-known structured log property names used by this package.
/// </summary>
public static class KnownLogPropertyNames
{
    public const string CorrelationId = "CorrelationId";
    public const string RequestId = "RequestId";
    public const string TraceId = "TraceId";
    public const string UserId = "UserId";
    public const string TenantId = "TenantId";
    public const string OperationName = "OperationName";
    public const string EntityType = "EntityType";
    public const string EntityId = "EntityId";
}
