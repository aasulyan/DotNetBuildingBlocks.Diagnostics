namespace DotNetBuildingBlocks.Serilog.Internal;

internal static class ArgumentGuard
{
    public static void ThrowIfNull(object? value, string paramName)
    {
        ArgumentNullException.ThrowIfNull(value, paramName);
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
        }
    }
}
