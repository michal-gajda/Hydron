namespace Hydron.Domain.Entities;

using Hydron.Domain.Events;
using Hydron.Domain.Exceptions;

public sealed class OrderEntity
{
    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Created;
    public int Version { get; private set; }
    private readonly List<object> domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => this.domainEvents.AsReadOnly();

    public OrderEntity(OrderId id, CustomerId customerId) : this(id, customerId, OrderStatus.Created, 1)
    {
    }

    internal OrderEntity(OrderId id, CustomerId customerId, OrderStatus status, int version)
    {
        Guard.AgainstDefault(id);
        Guard.AgainstDefault(customerId);

        if (version <= 0)
        {
            throw new InvalidDomainStateException("Order version must be greater than zero.");
        }

        if (!Enum.IsDefined(status))
        {
            throw new InvalidDomainStateException($"Unsupported order status '{status}'.");
        }

        this.Id = id;
        this.CustomerId = customerId;
        this.Status = status;
        this.Version = version;
    }

    public void ConfirmPayment()
    {
        this.EnsureStatus(OrderStatus.Created, nameof(ConfirmPayment));

        this.Status = OrderStatus.Confirmed;

        this.AddDomainEvent(new OrderConfirmedDomainEvent { Id = this.Id, });
    }

    public void Dispatch()
    {
        this.EnsureStatus(OrderStatus.Confirmed, nameof(Dispatch));

        this.Status = OrderStatus.Fulfilled;

        this.AddDomainEvent(new OrderDispatchedDomainEvent { Id = this.Id });
    }

    public void DeliverOrder()
    {
        this.EnsureStatus(OrderStatus.Fulfilled, nameof(DeliverOrder));

        this.Status = OrderStatus.Finalized;
    }

    public void Cancel()
    {
        if (this.Status is OrderStatus.Fulfilled or OrderStatus.Finalized or OrderStatus.Cancelled)
        {
            throw new InvalidDomainStateException($"Cannot cancel order in status {this.Status}.");
        }

        var oldStatus = this.Status;
        this.Status = OrderStatus.Cancelled;

        this.AddDomainEvent(new OrderCancelledDomainEvent { Id = Id, OldStatus = oldStatus, });
    }

    public void ClearDomainEvents() => this.domainEvents.Clear();

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        if (domainEvent is null)
        {
            throw new ArgumentNullException(nameof(domainEvent));
        }

        this.domainEvents.Add(domainEvent);
    }

    private void EnsureStatus(OrderStatus expectedStatus, string actionName)
    {
        if (this.Status != expectedStatus)
        {
            throw new InvalidDomainStateException($"Cannot perform action '{actionName}' because the order is in '{this.Status}' state instead of '{expectedStatus}'.");
        }
    }
}
