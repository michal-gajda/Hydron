namespace Hydron.Telemetry;

using System.Diagnostics;
using Hydron.Domain.Entities;
using Hydron.Domain.Events;
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

    public static void RecordOrderTimeToDispatch(OrderEntity order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var createdAtUtc = order.DomainEvents
            .OfType<OrderCreatedDomainEvent>()
            .Select(domainEvent => domainEvent.AddedAtUtc)
            .DefaultIfEmpty(default)
            .Min();

        var dispatchedAtUtc = order.DomainEvents
            .OfType<OrderDispatchedDomainEvent>()
            .Select(domainEvent => domainEvent.AddedAtUtc)
            .DefaultIfEmpty(default)
            .Min();

        if (createdAtUtc == default || dispatchedAtUtc == default)
        {
            return;
        }

        var seconds = (dispatchedAtUtc - createdAtUtc).TotalSeconds;
        if (seconds < 0)
        {
            seconds = 0;
        }

        OrdersMetrics.OrderTimeToDispatchHistogram.Record(
            seconds,
            new KeyValuePair<string, object?>("order.operation", "dispatch"),
            new KeyValuePair<string, object?>("order.status", order.Status.ToString()));
    }

    public static void RecordOrderTimeToCompletion(OrderEntity order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var createdAtUtc = order.DomainEvents
            .OfType<OrderCreatedDomainEvent>()
            .Select(domainEvent => domainEvent.AddedAtUtc)
            .DefaultIfEmpty(default)
            .Min();

        var deliveredAtUtc = order.DomainEvents
            .OfType<OrderDeliveredDomainEvent>()
            .Select(domainEvent => domainEvent.AddedAtUtc)
            .DefaultIfEmpty(default)
            .Min();

        var cancelledAtUtc = order.DomainEvents
            .OfType<OrderCancelledDomainEvent>()
            .Select(domainEvent => domainEvent.AddedAtUtc)
            .DefaultIfEmpty(default)
            .Min();

        if (createdAtUtc == default)
        {
            return;
        }

        DateTimeOffset completedAtUtc;
        string completionType;

        if (deliveredAtUtc == default && cancelledAtUtc == default)
        {
            return;
        }

        if (deliveredAtUtc == default || (cancelledAtUtc != default && cancelledAtUtc < deliveredAtUtc))
        {
            completedAtUtc = cancelledAtUtc;
            completionType = "cancel";
        }
        else
        {
            completedAtUtc = deliveredAtUtc;
            completionType = "deliver";
        }

        var seconds = (completedAtUtc - createdAtUtc).TotalSeconds;
        if (seconds < 0)
        {
            seconds = 0;
        }

        OrdersMetrics.OrderTimeToCompletionHistogram.Record(
            seconds,
            new KeyValuePair<string, object?>("order.operation", "completion"),
            new KeyValuePair<string, object?>("order.completion_type", completionType),
            new KeyValuePair<string, object?>("order.status", order.Status.ToString()));
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