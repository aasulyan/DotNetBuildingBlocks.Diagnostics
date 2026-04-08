using DotNetBuildingBlocks.Metrics.Internal;

namespace DotNetBuildingBlocks.Metrics.Tags;

/// <summary>
/// Creates common metric tags using stable tag names.
/// </summary>
public static class MetricTags
{
    public static KeyValuePair<string, object?> Operation(string operation)
        => CreateRequiredStringTag(MetricTagNames.Operation, operation, nameof(operation));

    public static KeyValuePair<string, object?> Outcome(string outcome)
        => CreateRequiredStringTag(MetricTagNames.Outcome, outcome, nameof(outcome));

    public static KeyValuePair<string, object?> TenantId(string tenantId)
        => CreateRequiredStringTag(MetricTagNames.TenantId, tenantId, nameof(tenantId));

    public static KeyValuePair<string, object?> UserId(string userId)
        => CreateRequiredStringTag(MetricTagNames.UserId, userId, nameof(userId));

    public static KeyValuePair<string, object?> EntityType(string entityType)
        => CreateRequiredStringTag(MetricTagNames.EntityType, entityType, nameof(entityType));

    public static KeyValuePair<string, object?> EntityId(object entityId)
    {
        ArgumentNullException.ThrowIfNull(entityId);
        return new KeyValuePair<string, object?>(MetricTagNames.EntityId, entityId);
    }

    public static KeyValuePair<string, object?> DependencySystem(string dependencySystem)
        => CreateRequiredStringTag(MetricTagNames.DependencySystem, dependencySystem, nameof(dependencySystem));

    public static KeyValuePair<string, object?> ErrorType(string errorType)
        => CreateRequiredStringTag(MetricTagNames.ErrorType, errorType, nameof(errorType));

    public static KeyValuePair<string, object?> Status(string status)
        => CreateRequiredStringTag(MetricTagNames.Status, status, nameof(status));

    public static KeyValuePair<string, object?> MessageType(string messageType)
        => CreateRequiredStringTag(MetricTagNames.MessageType, messageType, nameof(messageType));

    public static KeyValuePair<string, object?> Create(string name, object? value)
    {
        var tagName = ArgumentGuard.NotNullOrWhiteSpace(name, nameof(name));
        return new KeyValuePair<string, object?>(tagName, value);
    }

    private static KeyValuePair<string, object?> CreateRequiredStringTag(string tagName, string value, string paramName)
    {
        var tagValue = ArgumentGuard.NotNullOrWhiteSpace(value, paramName);
        return new KeyValuePair<string, object?>(tagName, tagValue);
    }
}
