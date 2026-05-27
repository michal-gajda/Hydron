namespace Hydron.Application.Orders.CommandHandlers;

using Hydron.Application.Orders.Commands;
using Hydron.Domain.Exceptions;
using Hydron.Domain.Interfaces;

internal sealed class CancelOrderHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrder, Result>
{
    public async Task<Result> Handle(CancelOrder request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.LoadAsync(request.Id, cancellationToken);

        if (order is null)
        {
            return Result.Fail(OrderErrors.NotFound(request.Id));
        }

        try
        {
            order.Cancel();
            await orderRepository.SaveAsync(order, cancellationToken);

            return Result.Ok();
        }
        catch (InvalidDomainStateException exception)
        {
            return Result.Fail(OrderErrors.InvalidState(exception.Message));
        }
    }
}