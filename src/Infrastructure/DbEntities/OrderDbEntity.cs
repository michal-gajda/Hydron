namespace Hydron.Infrastructure.DbEntities;

using Hydron.Domain.Entities;
using Hydron.Domain.Enums;
using Hydron.Domain.ValueObjects;

internal sealed class OrderDbEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public int Version { get; set; }

    public static explicit operator OrderDbEntity(OrderEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new OrderDbEntity
        {
            Id = entity.Id.Value,
            CustomerId = entity.CustomerId.Value,
            Status = entity.Status,
            Version = entity.Version,
        };
    }

    public static explicit operator OrderEntity(OrderDbEntity dbEntity)
    {
        ArgumentNullException.ThrowIfNull(dbEntity);

        return new OrderEntity(
            id: new OrderId(dbEntity.Id),
            customerId: new CustomerId(dbEntity.CustomerId),
            status: dbEntity.Status,
            version: dbEntity.Version);
    }
}