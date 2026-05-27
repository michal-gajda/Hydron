namespace Hydron.Infrastructure.Services;

using Hydron.Domain.Entities;
using Hydron.Domain.Events;
using Hydron.Domain.Interfaces;
using Hydron.Domain.ValueObjects;
using Hydron.Infrastructure.DbEntities;
using MediatR;

internal sealed class OrderRepository(IPublisher publisher) : IOrderRepository
{
    private readonly Dictionary<OrderId, OrderDbEntity> orders = new();
    private readonly Lock sync = new();

    public Task<OrderEntity?> LoadAsync(OrderId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (this.sync)
        {
            this.orders.TryGetValue(id, out var dbEntity);

            return Task.FromResult(dbEntity is null ? null : (OrderEntity)dbEntity);
        }
    }

    public async Task SaveAsync(OrderEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(entity);

        List<DomainEvent> unpublishedEvents;

        lock (this.sync)
        {
            this.orders[entity.Id] = (OrderDbEntity)entity;
            unpublishedEvents = entity.UnpublishedDomainEvents.ToList();
        }

        foreach (var domainEvent in unpublishedEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
            entity.MarkDomainEventAsPublished(domainEvent);

            lock (this.sync)
            {
                this.orders[entity.Id] = (OrderDbEntity)entity;
            }
        }
    }
}
