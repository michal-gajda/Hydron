namespace Hydron.Application.Telemetry;

using System.Diagnostics;

public static class HydronTelemetry
{
    public const string ServiceVersion = "1.0.0";
    public const string ActivitySourceName = "Hydron.Application";
    public const string MeterName = "Hydron.Application";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, ServiceVersion);
}