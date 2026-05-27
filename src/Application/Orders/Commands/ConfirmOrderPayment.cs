namespace Hydron.Application.Orders.Commands;

public sealed record ConfirmOrderPayment : IRequest<Result>
{
    public required OrderId Id { get; init; }
}