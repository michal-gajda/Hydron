namespace Hydron.Domain.Events;

public sealed record OrderDispatchedDomainEvent : DomainEvent
{
    public required OrderId Id { get; init; }
}
