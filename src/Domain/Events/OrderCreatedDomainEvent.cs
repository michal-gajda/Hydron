namespace Hydron.Domain.Events;

public sealed record OrderCreatedDomainEvent : DomainEvent
{
    public required OrderId Id { get; init; }
}