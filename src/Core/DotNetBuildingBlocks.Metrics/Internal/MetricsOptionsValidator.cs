using Microsoft.Extensions.Options;
using DotNetBuildingBlocks.Metrics.Options;

namespace DotNetBuildingBlocks.Metrics.Internal;

internal sealed class MetricsOptionsValidator : IValidateOptions<MetricsOptions>
{
    public ValidateOptionsResult Validate(string? name, MetricsOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail("MetricsOptions cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(options.MeterName))
        {
            return ValidateOptionsResult.Fail("MetricsOptions.MeterName cannot be null or whitespace.");
        }

        return ValidateOptionsResult.Success;
    }
}
