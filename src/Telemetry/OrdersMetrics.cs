namespace Hydron.Telemetry;

using System.Diagnostics.Metrics;

public static class OrdersMetrics
{
    public const string OrdersCreatedCounterName = "hydron.orders.created";
    public const string OrderTimeToDispatchHistogramName = "hydron.orders.time_to_dispatch";
    public const string OrderTimeToCompletionHistogramName = "hydron.orders.time_to_completion";

    private static readonly Meter Meter = new(HydronTelemetry.MeterName, HydronTelemetry.ServiceVersion);

    public static readonly Counter<long> OrdersCreatedCounter = Meter.CreateCounter<long>(
        OrdersCreatedCounterName,
        unit: "{order}",
        description: "Number of successfully created orders.");

    public static readonly Histogram<double> OrderTimeToDispatchHistogram = Meter.CreateHistogram<double>(
        OrderTimeToDispatchHistogramName,
        unit: "s",
        description: "Time between order creation and dispatch in seconds.");

    public static readonly Histogram<double> OrderTimeToCompletionHistogram = Meter.CreateHistogram<double>(
        OrderTimeToCompletionHistogramName,
        unit: "s",
        description: "Time between order creation and completion (delivery or cancellation) in seconds.");
}
