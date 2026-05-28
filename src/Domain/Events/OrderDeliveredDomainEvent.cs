namespace Hydron.Domain.Events;

public sealed record OrderDeliveredDomainEvent : DomainEvent
{
    public required OrderId Id { get; init; }
}
