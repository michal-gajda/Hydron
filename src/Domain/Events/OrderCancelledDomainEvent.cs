namespace Hydron.Domain.Events;

public sealed record OrderCancelledDomainEvent : DomainEvent
{
    public required OrderId Id { get; init; }
    public required OrderStatus OldStatus { get; init; }
}
