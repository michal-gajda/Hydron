namespace Hydron.Application.Orders.Exceptions;

public sealed class OrderNotFoundException(OrderId orderId) : ApplicationException($"Order '{orderId}' was not found.")
{
    public override string ErrorCode { get; protected set; } = "order_not_found";
    public OrderId OrderId { get; } = orderId;
}
