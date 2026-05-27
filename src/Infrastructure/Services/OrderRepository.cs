namespace Hydron.Infrastructure.Services;

using Hydron.Domain.Entities;
using Hydron.Domain.Interfaces;
using Hydron.Domain.ValueObjects;
using Hydron.Infrastructure.DbEntities;

internal sealed class OrderRepository : IOrderRepository
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

    public Task SaveAsync(OrderEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(entity);

        lock (this.sync)
        {
            this.orders[entity.Id] = (OrderDbEntity)entity;
        }

        return Task.CompletedTask;
    }
}
