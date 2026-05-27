namespace Hydron.Domain.Events;

public abstract record class DomainEvent : INotification
{
    public DateTimeOffset AddedAtUtc { get; internal set; }
    public bool IsPublished { get; internal set; }
}
