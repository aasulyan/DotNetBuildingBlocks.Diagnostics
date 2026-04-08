using System.Collections.Generic;
using DotNetBuildingBlocks.Tracing.Abstractions;
using DotNetBuildingBlocks.Tracing.Internal;
using DotNetBuildingBlocks.Tracing.Options;
using Microsoft.Extensions.Options;

namespace DotNetBuildingBlocks.Tracing.Activities;

/// <summary>
/// Default implementation of <see cref="IActivitySourceAccessor"/>.
/// </summary>
public sealed class ActivitySourceAccessor : IActivitySourceAccessor, IDisposable
{
    private readonly ActivitySource activitySource;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivitySourceAccessor"/> class.
    /// </summary>
    /// <param name="options">Tracing options.</param>
    public ActivitySourceAccessor(IOptions<TracingOptions> options)
    {
        ArgumentGuard.ThrowIfNull(options, nameof(options));

        var tracingOptions = options.Value;
        ArgumentGuard.ThrowIfNullOrWhiteSpace(tracingOptions.ActivitySourceName, nameof(tracingOptions.ActivitySourceName));

        activitySource = new ActivitySource(tracingOptions.ActivitySourceName, tracingOptions.ActivitySourceVersion);
    }

    /// <inheritdoc />
    public ActivitySource ActivitySource => activitySource;

    /// <inheritdoc />
    public Activity? StartActivity(
        string name,
        ActivityKind kind = ActivityKind.Internal,
        ActivityContext parentContext = default,
        IEnumerable<KeyValuePair<string, object?>>? tags = null,
        IEnumerable<ActivityLink>? links = null)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentGuard.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return activitySource.StartActivity(name, kind, parentContext, tags, links);
    }

    /// <inheritdoc />
    public Activity? StartInternalActivity(
        string name,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        return StartActivity(name, ActivityKind.Internal, default, tags);
    }

    /// <inheritdoc />
    public Activity? StartClientActivity(
        string name,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        return StartActivity(name, ActivityKind.Client, default, tags);
    }

    /// <inheritdoc />
    public Activity? StartServerActivity(
        string name,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        return StartActivity(name, ActivityKind.Server, default, tags);
    }

    /// <inheritdoc />
    public Activity? StartProducerActivity(
        string name,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        return StartActivity(name, ActivityKind.Producer, default, tags);
    }

    /// <inheritdoc />
    public Activity? StartConsumerActivity(
        string name,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        return StartActivity(name, ActivityKind.Consumer, default, tags);
    }

    /// <summary>
    /// Disposes the owned <see cref="ActivitySource"/>.
    /// </summary>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        activitySource.Dispose();
        disposed = true;
    }
}
