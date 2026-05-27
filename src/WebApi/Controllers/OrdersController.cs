namespace Hydron.WebApi.Controllers;

using FluentResults;
using Hydron.Application.Orders.Commands;
using Hydron.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("orders")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
        {
            return this.BadRequest(new { errors = new[] { "CustomerId cannot be empty." } });
        }

        var orderId = new OrderId(Guid.NewGuid());

        var result = await sender.Send(new CreateOrder
        {
            Id = orderId,
            CustomerId = new CustomerId(request.CustomerId),
        }, cancellationToken);

        var failureResult = this.ToFailureResult(result);

        if (failureResult is not null)
        {
            return failureResult;
        }

        var response = new CreateOrderResponse(orderId.Value, request.CustomerId);

        return this.Created($"/orders/{response.OrderId}", response);
    }

    [HttpPost("{orderId:guid}/confirm-payment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmPaymentAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var parsedOrderId = this.TryCreateOrderId(orderId, out var badRequestResult);

        if (badRequestResult is not null)
        {
            return badRequestResult;
        }

        var result = await sender.Send(new ConfirmOrderPayment { Id = parsedOrderId!.Value }, cancellationToken);

        return this.ToStateChangeResult(result);
    }

    [HttpPost("{orderId:guid}/dispatch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DispatchAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var parsedOrderId = this.TryCreateOrderId(orderId, out var badRequestResult);

        if (badRequestResult is not null)
        {
            return badRequestResult;
        }

        var result = await sender.Send(new DispatchOrder { Id = parsedOrderId!.Value }, cancellationToken);

        return this.ToStateChangeResult(result);
    }

    [HttpPost("{orderId:guid}/deliver")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeliverAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var parsedOrderId = this.TryCreateOrderId(orderId, out var badRequestResult);

        if (badRequestResult is not null)
        {
            return badRequestResult;
        }

        var result = await sender.Send(new DeliverOrder { Id = parsedOrderId!.Value }, cancellationToken);

        return this.ToStateChangeResult(result);
    }

    [HttpPost("{orderId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var parsedOrderId = this.TryCreateOrderId(orderId, out var badRequestResult);

        if (badRequestResult is not null)
        {
            return badRequestResult;
        }

        var result = await sender.Send(new CancelOrder { Id = parsedOrderId!.Value }, cancellationToken);

        return this.ToStateChangeResult(result);
    }

    private IActionResult ToStateChangeResult(Result result)
    {
        var failureResult = this.ToFailureResult(result);

        return failureResult ?? this.NoContent();
    }

    private IActionResult? ToFailureResult(Result result)
    {
        if (!result.IsFailed)
        {
            return null;
        }

        var errors = result.Errors.Select(error => error.Message).ToArray();
        var hasNotFoundError = result.Errors.Any(error => error.Metadata.TryGetValue("code", out var code) && Equals(code, "not_found"));

        return hasNotFoundError
            ? this.NotFound(new { errors })
            : this.BadRequest(new { errors });
    }

    private OrderId? TryCreateOrderId(Guid orderId, out IActionResult? badRequestResult)
    {
        if (orderId == Guid.Empty)
        {
            badRequestResult = this.BadRequest(new { errors = new[] { "OrderId cannot be empty." } });
            return null;
        }

        badRequestResult = null;

        return new OrderId(orderId);
    }
}

public sealed record CreateOrderRequest
{
    public required Guid CustomerId { get; init; }
}

public sealed record CreateOrderResponse(Guid OrderId, Guid CustomerId);