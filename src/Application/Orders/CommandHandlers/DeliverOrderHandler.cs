namespace Hydron.Application.Orders.CommandHandlers;

using Hydron.Application.Orders.Commands;
using Hydron.Domain.Exceptions;
using Hydron.Domain.Interfaces;

internal sealed class DeliverOrderHandler(IOrderRepository orderRepository) : IRequestHandler<DeliverOrder, Result>
{
    public async Task<Result> Handle(DeliverOrder request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.LoadAsync(request.Id, cancellationToken);

        if (order is null)
        {
            return Result.Fail(OrderErrors.NotFound(request.Id));
        }

        try
        {
            order.DeliverOrder();
            await orderRepository.SaveAsync(order, cancellationToken);

            return Result.Ok();
        }
        catch (InvalidDomainStateException exception)
        {
            return Result.Fail(OrderErrors.InvalidState(exception.Message));
        }
    }
}