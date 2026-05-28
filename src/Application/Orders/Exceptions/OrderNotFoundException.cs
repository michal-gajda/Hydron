namespace Hydron.Application.Orders.Exceptions;

public sealed class OrderNotFoundException(OrderId orderId) : ApplicationException($"Order '{orderId}' was not found.")
{
    public OrderId OrderId { get; } = orderId;
}
