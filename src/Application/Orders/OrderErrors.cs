namespace Hydron.Application.Orders;

using FluentResults;

internal static class OrderErrors
{
    public static Error NotFound(OrderId orderId)
    {
        return new Error($"Order '{orderId}' was not found.")
            .WithMetadata("code", "not_found");
    }

    public static Error InvalidState(string message)
    {
        return new Error(message)
            .WithMetadata("code", "invalid_state");
    }
}