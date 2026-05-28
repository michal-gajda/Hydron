namespace Hydron.Application.Orders.CommandHandlers;

using Hydron.Application.Orders.Commands;
using Hydron.Application.Orders.Exceptions;
using Hydron.Domain.Interfaces;
using Hydron.Telemetry;

internal sealed class DeliverOrderHandler(IOrderRepository orderRepository, TimeProvider timeProvider) : IRequestHandler<DeliverOrder, Result>
{
    public async Task<Result> Handle(DeliverOrder request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.LoadAsync(request.Id, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(request.Id);
        }

        var createdAtUtc = timeProvider.GetUtcNow();
        order.DeliverOrder(createdAtUtc);
        await orderRepository.SaveAsync(order, cancellationToken);

        OrdersTelemetry.RecordOrderTimeToCompletion(order);

        return Result.Ok();
    }
}