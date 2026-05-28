namespace Hydron.Application.Orders.CommandHandlers;

using Hydron.Application.Orders.Commands;
using Hydron.Domain.Exceptions;
using Hydron.Domain.Interfaces;
using Hydron.Telemetry;

internal sealed class DispatchOrderHandler(IOrderRepository orderRepository, TimeProvider timeProvider) : IRequestHandler<DispatchOrder, Result>
{
    public async Task<Result> Handle(DispatchOrder request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.LoadAsync(request.Id, cancellationToken);

        if (order is null)
        {
            return Result.Fail(OrderErrors.NotFound(request.Id));
        }

        try
        {
            var createdAtUtc = timeProvider.GetUtcNow();
            order.Dispatch(createdAtUtc);
            await orderRepository.SaveAsync(order, cancellationToken);

            OrdersTelemetry.RecordOrderTimeToDispatch(order);

            return Result.Ok();
        }
        catch (InvalidDomainStateException exception)
        {
            return Result.Fail(OrderErrors.InvalidState(exception.Message));
        }
    }
}