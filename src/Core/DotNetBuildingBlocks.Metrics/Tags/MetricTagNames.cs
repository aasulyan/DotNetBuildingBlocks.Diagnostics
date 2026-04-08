namespace DotNetBuildingBlocks.Metrics.Tags;

/// <summary>
/// Well-known metric tag names used by the package.
/// </summary>
public static class MetricTagNames
{
    public const string Operation = "operation";
    public const string Outcome = "outcome";
    public const string TenantId = "tenant.id";
    public const string UserId = "user.id";
    public const string EntityType = "entity.type";
    public const string EntityId = "entity.id";
    public const string DependencySystem = "dependency.system";
    public const string ErrorType = "error.type";
    public const string Status = "status";
    public const string MessageType = "message.type";
}
