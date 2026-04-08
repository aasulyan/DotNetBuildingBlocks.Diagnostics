using DotNetBuildingBlocks.Observability.Options;
using Microsoft.Extensions.Options;

namespace DotNetBuildingBlocks.Observability.Internal;

internal sealed class ObservabilityOptionsValidator : IValidateOptions<ObservabilityOptions>
{
    public ValidateOptionsResult Validate(string? name, ObservabilityOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            return ValidateOptionsResult.Fail("ObservabilityOptions.ServiceName cannot be null or whitespace.");
        }

        if (options.Exporters.Otlp.Enabled &&
            options.Exporters.Otlp.Endpoint is not null &&
            !Uri.TryCreate(options.Exporters.Otlp.Endpoint, UriKind.Absolute, out _))
        {
            return ValidateOptionsResult.Fail(
                $"ObservabilityOptions.Exporters.Otlp.Endpoint '{options.Exporters.Otlp.Endpoint}' is not a valid absolute URI.");
        }

        return ValidateOptionsResult.Success;
    }
}
