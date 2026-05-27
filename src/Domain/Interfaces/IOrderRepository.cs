namespace Hydron.Domain.Interfaces;

using Hydron.Domain.Entities;

public interface IOrderRepository
{
    Task<OrderEntity?> LoadAsync(OrderId id, CancellationToken cancellationToken = default);
    Task SaveAsync(OrderEntity entity, CancellationToken cancellationToken = default);
}
