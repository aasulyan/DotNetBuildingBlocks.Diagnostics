using DotNetBuildingBlocks.Observation.Models;
using DotNetBuildingBlocks.Observation.Options;

namespace DotNetBuildingBlocks.Observation.Internal;

internal static class ObservationIdentityResolver
{
    public static ObservationIdentity Resolve(ObservationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new ArgumentException("ObservationOptions.ServiceName cannot be null or whitespace.", nameof(options));
        }

        var serviceName = options.ServiceName.Trim();
        var activitySourceName = string.IsNullOrWhiteSpace(options.ActivitySourceName)
            ? serviceName
            : options.ActivitySourceName.Trim();
        var meterName = string.IsNullOrWhiteSpace(options.MeterName)
            ? serviceName
            : options.MeterName.Trim();
        var serviceVersion = string.IsNullOrWhiteSpace(options.ServiceVersion)
            ? null
            : options.ServiceVersion.Trim();

        return new ObservationIdentity(serviceName, serviceVersion, activitySourceName, meterName);
    }
}
