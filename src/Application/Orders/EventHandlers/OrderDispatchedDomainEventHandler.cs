namespace Hydron.Application.Orders.EventHandlers;

using Hydron.Domain.Events;

internal sealed partial class OrderDispatchedDomainEventHandler(ILogger<OrderDispatchedDomainEventHandler> logger) : INotificationHandler<OrderDispatchedDomainEvent>
{
    public Task Handle(OrderDispatchedDomainEvent notification, CancellationToken cancellationToken)
    {
        LogOrderDispatched(notification.Id.ToString());

        return Task.CompletedTask;
    }

    [LoggerMessage(EventId = 7101, Level = LogLevel.Information, Message = "Order {orderId} was dispatched (domain event).")]
    private partial void LogOrderDispatched(string orderId);
}
