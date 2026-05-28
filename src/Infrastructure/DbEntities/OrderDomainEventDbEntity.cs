namespace Hydron.Infrastructure.DbEntities;

internal sealed class OrderDomainEventDbEntity
{
    public required string Type { get; init; }
    public DateTimeOffset AddedAtUtc { get; init; }
    public OrderStatus? OldStatus { get; init; }
    public bool IsPublished { get; init; }
}
