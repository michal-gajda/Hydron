namespace Hydron.Telemetry;

using System.Diagnostics.Metrics;

public static class OrdersMetrics
{
    public const string OrdersCreatedCounterName = "hydron.orders.created";

    private static readonly Meter Meter = new(HydronTelemetry.MeterName, HydronTelemetry.ServiceVersion);

    public static readonly Counter<long> OrdersCreatedCounter = Meter.CreateCounter<long>(
        OrdersCreatedCounterName,
        unit: "{order}",
        description: "Number of successfully created orders.");
}
