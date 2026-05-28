namespace Hydron.Application.Orders.CommandHandlers;

using Hydron.Application.Orders.Commands;
using Hydron.Application.Orders.Exceptions;
using Hydron.Domain.Interfaces;

internal sealed class CancelOrderHandler(IOrderRepository orderRepository, TimeProvider timeProvider) : IRequestHandler<CancelOrder, Result>
{
    public async Task<Result> Handle(CancelOrder request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.LoadAsync(request.Id, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(request.Id);
        }

        var createdAtUtc = timeProvider.GetUtcNow();
        order.Cancel(createdAtUtc);
        await orderRepository.SaveAsync(order, cancellationToken);

        return Result.Ok();
    }
}