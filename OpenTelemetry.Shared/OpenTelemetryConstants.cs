namespace OpenTelemetry.Shared;

public sealed class OpenTelemetryConstants
{
    public string ServiceName { get; set; } = null!;
    public string ServiceVersion { get; set; } = null!;
    public string ActivitySourceName { get; set; } = null!;
    public string OtelCollectorBaseUrl { get; set; } = null!;
}