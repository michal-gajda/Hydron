namespace Hydron.Application.Orders.Commands;

public sealed record DispatchOrder : IRequest<Result>
{
    public required OrderId Id { get; init; }
}