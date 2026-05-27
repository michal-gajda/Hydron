namespace Hydron.Domain.Events;

public sealed record OrderConfirmedDomainEvent : DomainEvent
{
    public required OrderId Id { get; init; }
}
