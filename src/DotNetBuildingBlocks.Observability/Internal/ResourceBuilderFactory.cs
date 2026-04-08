using DotNetBuildingBlocks.Observability.Options;
using OpenTelemetry.Resources;

namespace DotNetBuildingBlocks.Observability.Internal;

internal static class ResourceBuilderFactory
{
    public static ResourceBuilder Create(ObservabilityOptions options, string activitySourceName, string meterName)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: options.ServiceName.Trim(),
                serviceVersion: string.IsNullOrWhiteSpace(options.ServiceVersion) ? null : options.ServiceVersion.Trim(),
                serviceNamespace: options.Resource.ServiceNamespace,
                serviceInstanceId: options.Resource.ServiceInstanceId);

        if (!string.IsNullOrWhiteSpace(options.Resource.DeploymentEnvironment))
        {
            resourceBuilder.AddAttributes(new KeyValuePair<string, object>[]
            {
                new("deployment.environment", options.Resource.DeploymentEnvironment!)
            });
        }

        if (options.Resource.Attributes.Count > 0)
        {
            resourceBuilder.AddAttributes(options.Resource.Attributes);
        }

        return resourceBuilder;
    }
}
