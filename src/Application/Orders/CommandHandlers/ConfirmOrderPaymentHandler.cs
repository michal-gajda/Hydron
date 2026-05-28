namespace Hydron.Application.Orders.CommandHandlers;

using Hydron.Application.Orders.Commands;
using Hydron.Application.Orders.Exceptions;
using Hydron.Domain.Interfaces;

internal sealed class ConfirmOrderPaymentHandler(IOrderRepository orderRepository, TimeProvider timeProvider) : IRequestHandler<ConfirmOrderPayment, Result>
{
    public async Task<Result> Handle(ConfirmOrderPayment request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.LoadAsync(request.Id, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(request.Id);
        }

        var createdAtUtc = timeProvider.GetUtcNow();
        order.ConfirmPayment(createdAtUtc);
        await orderRepository.SaveAsync(order, cancellationToken);

        return Result.Ok();
    }
}