using DotNetBuildingBlocks.Serilog.Options;

namespace DotNetBuildingBlocks.Serilog.Internal;

/// <summary>
/// Validates <see cref="DotNetSerilogOptions"/> instances and exposes a single entry point used by the
/// host and service collection registration extensions.
/// </summary>
internal static class SerilogOptionsValidator
{
    public static void Validate(DotNetSerilogOptions options)
    {
        ArgumentGuard.ThrowIfNull(options, nameof(options));

        if (string.IsNullOrWhiteSpace(options.ApplicationName))
        {
            throw new ArgumentException(
                "DotNetSerilogOptions.ApplicationName cannot be null, empty, or whitespace.",
                nameof(options));
        }

        if (options.ReadFromConfiguration && string.IsNullOrWhiteSpace(options.ConfigurationSectionName))
        {
            throw new ArgumentException(
                "DotNetSerilogOptions.ConfigurationSectionName cannot be null or whitespace when ReadFromConfiguration is true.",
                nameof(options));
        }
    }
}
