namespace Hydron.Application.Orders.Commands;

public sealed record DeliverOrder : IRequest<Result>
{
    public required OrderId Id { get; init; }
}