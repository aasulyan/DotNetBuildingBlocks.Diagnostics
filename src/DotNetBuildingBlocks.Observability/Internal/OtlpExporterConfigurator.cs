using DotNetBuildingBlocks.Observability.Options;
using OpenTelemetry.Exporter;

namespace DotNetBuildingBlocks.Observability.Internal;

/// <summary>
/// Applies <see cref="OtlpOptions"/> configuration to the underlying OpenTelemetry <see cref="OtlpExporterOptions"/>.
/// Shared between tracing and metrics pipelines.
/// </summary>
internal static class OtlpExporterConfigurator
{
    public static void Apply(OtlpExporterOptions otlp, OtlpOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            otlp.Endpoint = new Uri(options.Endpoint);
        }

        if (!string.IsNullOrWhiteSpace(options.Protocol))
        {
            otlp.Protocol = options.Protocol.Equals("grpc", StringComparison.OrdinalIgnoreCase)
                ? OtlpExportProtocol.Grpc
                : OtlpExportProtocol.HttpProtobuf;
        }

        if (options.Headers.Count > 0)
        {
            otlp.Headers = string.Join(",", options.Headers.Select(h => $"{h.Key}={h.Value}"));
        }
    }
}
