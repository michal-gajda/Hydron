namespace Hydron.Application.Orders.Commands;

public sealed record CancelOrder : IRequest<Result>
{
    public required OrderId Id { get; init; }
}