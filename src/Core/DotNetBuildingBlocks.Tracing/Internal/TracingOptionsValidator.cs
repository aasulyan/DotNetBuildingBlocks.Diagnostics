using DotNetBuildingBlocks.Tracing.Options;
using Microsoft.Extensions.Options;

namespace DotNetBuildingBlocks.Tracing.Internal;

internal sealed class TracingOptionsValidator : IValidateOptions<TracingOptions>
{
    public ValidateOptionsResult Validate(string? name, TracingOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ActivitySourceName))
        {
            return ValidateOptionsResult.Fail("TracingOptions.ActivitySourceName cannot be null or whitespace.");
        }

        return ValidateOptionsResult.Success;
    }
}
