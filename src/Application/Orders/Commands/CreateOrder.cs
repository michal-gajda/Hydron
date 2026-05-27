namespace Hydron.Application.Orders.Commands;

public sealed record CreateOrder : IRequest<Result>
{
    public required CustomerId CustomerId { get; init; }
    public required OrderId Id { get; init; }
}
