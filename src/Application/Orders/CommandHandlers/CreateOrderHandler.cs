namespace Hydron.Application.Orders.CommandHandlers;

using Hydron.Application.Orders.Commands;
using Hydron.Application.Telemetry;
using Hydron.Domain.Entities;
using Hydron.Domain.Interfaces;

internal sealed class CreateOrderHandler(IOrderRepository orderRepository) : IRequestHandler<CreateOrder, Result>
{
    public async Task<Result> Handle(CreateOrder request, CancellationToken cancellationToken)
    {
        using var activity = OrdersTelemetry.StartCreateOrderActivity(request.Id, request.CustomerId);

        try
        {
            var entity = new OrderEntity(request.Id, request.CustomerId);
            await orderRepository.SaveAsync(entity, cancellationToken);

            OrdersTelemetry.RecordCreateOrderSuccess(request.CustomerId);
            OrdersTelemetry.MarkSuccess(activity);

            return Result.Ok();
        }
        catch (Exception exception)
        {
            OrdersTelemetry.MarkFailure(activity, exception);
            throw;
        }
    }
}
