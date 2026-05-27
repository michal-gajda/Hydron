namespace Hydron.Infrastructure.DbEntities;

using Hydron.Domain.Entities;
using Hydron.Domain.Enums;
using Hydron.Domain.Events;
using Hydron.Domain.ValueObjects;

internal sealed class OrderDbEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public int Version { get; set; }
    public List<OrderDomainEventDbEntity> DomainEvents { get; set; } = [];

    public static explicit operator OrderDbEntity(OrderEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new OrderDbEntity
        {
            Id = entity.Id.Value,
            CustomerId = entity.CustomerId.Value,
            Status = entity.Status,
            Version = entity.Version,
            DomainEvents = entity.DomainEvents.Select(MapDomainEvent).ToList(),
        };
    }

    public static explicit operator OrderEntity(OrderDbEntity dbEntity)
    {
        ArgumentNullException.ThrowIfNull(dbEntity);

        return new OrderEntity(
            id: new OrderId(dbEntity.Id),
            customerId: new CustomerId(dbEntity.CustomerId),
            status: dbEntity.Status,
            version: dbEntity.Version,
            domainEvents: dbEntity.DomainEvents.Select(domainEvent => MapDbEvent(domainEvent, new OrderId(dbEntity.Id))).ToList());
    }

    private static OrderDomainEventDbEntity MapDomainEvent(DomainEvent domainEvent)
    {
        return domainEvent switch
        {
            OrderCreatedDomainEvent => new OrderDomainEventDbEntity { Type = nameof(OrderCreatedDomainEvent), AddedAtUtc = domainEvent.AddedAtUtc, IsPublished = domainEvent.IsPublished },
            OrderConfirmedDomainEvent => new OrderDomainEventDbEntity { Type = nameof(OrderConfirmedDomainEvent), AddedAtUtc = domainEvent.AddedAtUtc, IsPublished = domainEvent.IsPublished },
            OrderDispatchedDomainEvent => new OrderDomainEventDbEntity { Type = nameof(OrderDispatchedDomainEvent), AddedAtUtc = domainEvent.AddedAtUtc, IsPublished = domainEvent.IsPublished },
            OrderCancelledDomainEvent cancelled => new OrderDomainEventDbEntity { Type = nameof(OrderCancelledDomainEvent), AddedAtUtc = domainEvent.AddedAtUtc, OldStatus = cancelled.OldStatus, IsPublished = domainEvent.IsPublished },
            _ => throw new InvalidOperationException($"Unsupported domain event type '{domainEvent.GetType().Name}'."),
        };
    }

    private static DomainEvent MapDbEvent(OrderDomainEventDbEntity domainEvent, OrderId orderId)
    {
        DomainEvent mappedEvent = domainEvent.Type switch
        {
            nameof(OrderCreatedDomainEvent) => new OrderCreatedDomainEvent { Id = orderId },
            nameof(OrderConfirmedDomainEvent) => new OrderConfirmedDomainEvent { Id = orderId },
            nameof(OrderDispatchedDomainEvent) => new OrderDispatchedDomainEvent { Id = orderId },
            nameof(OrderCancelledDomainEvent) => new OrderCancelledDomainEvent { Id = orderId, OldStatus = domainEvent.OldStatus ?? throw new InvalidOperationException("OldStatus is required for cancellation events.") },
            _ => throw new InvalidOperationException($"Unsupported domain event type '{domainEvent.Type}'."),
        };

        mappedEvent.AddedAtUtc = domainEvent.AddedAtUtc;
        mappedEvent.IsPublished = domainEvent.IsPublished;

        return mappedEvent;
    }
}

internal sealed class OrderDomainEventDbEntity
{
    public required string Type { get; init; }
    public DateTimeOffset AddedAtUtc { get; init; }
    public OrderStatus? OldStatus { get; init; }
    public bool IsPublished { get; init; }
}