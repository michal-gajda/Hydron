namespace Hydron.Telemetry;

using System.Diagnostics;
using Hydron.Domain.ValueObjects;

public static class OrdersTelemetry
{
    public static Activity? StartCreateOrderActivity(OrderId orderId, CustomerId customerId)
    {
        var activity = HydronTelemetry.ActivitySource.StartActivity("orders.create", ActivityKind.Internal);
        activity?.SetTag("order.id", orderId.ToString());
        activity?.SetTag("customer.id", customerId.ToString());

        return activity;
    }

    public static void RecordCreateOrderSuccess(CustomerId customerId)
    {
        OrdersMetrics.OrdersCreatedCounter.Add(
            1,
            new KeyValuePair<string, object?>("order.operation", "create"),
            new KeyValuePair<string, object?>("order.customer_id", customerId.ToString()));
    }

    public static void MarkSuccess(Activity? activity)
    {
        activity?.SetStatus(ActivityStatusCode.Ok);
    }

    public static void MarkFailure(Activity? activity, Exception exception)
    {
        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
    }
}