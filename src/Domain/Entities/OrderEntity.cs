namespace Hydron.Domain.Entities;

using Hydron.Domain.Events;
using Hydron.Domain.Exceptions;

public sealed class OrderEntity
{
    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Created;
    public int Version { get; private set; }
    private readonly List<DomainEvent> domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => this.domainEvents.AsReadOnly();
    public DateTimeOffset LastModifiedAtUtc => this.domainEvents.Max(domainEvent => domainEvent.AddedAtUtc);
    internal IReadOnlyCollection<DomainEvent> UnpublishedDomainEvents => this.domainEvents.Where(domainEvent => !domainEvent.IsPublished).ToArray();

    public OrderEntity(OrderId id, CustomerId customerId, DateTimeOffset createdAtUtc)
        : this(id, customerId, OrderStatus.Created, 1, new[] { new OrderCreatedDomainEvent { Id = id, AddedAtUtc = createdAtUtc, IsPublished = false } })
    {
        if (createdAtUtc == default)
        {
            throw new InvalidDomainStateException("Order creation timestamp must be provided.");
        }
    }

    internal OrderEntity(OrderId id, CustomerId customerId, OrderStatus status, int version, IReadOnlyCollection<DomainEvent>? domainEvents)
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

        if (domainEvents is not null)
        {
            foreach (var domainEvent in domainEvents.OrderBy(domainEvent => domainEvent.AddedAtUtc))
            {
                if (domainEvent is null)
                {
                    throw new InvalidDomainStateException("Domain event cannot be null.");
                }

                if (domainEvent.AddedAtUtc == default)
                {
                    throw new InvalidDomainStateException("Domain event timestamp must be provided.");
                }

                this.domainEvents.Add(domainEvent);
            }
        }

        if (this.Status is OrderStatus.Fulfilled or OrderStatus.Finalized && this.domainEvents.All(domainEvent => domainEvent is not OrderDispatchedDomainEvent))
        {
            throw new InvalidDomainStateException("Dispatch event must be present for fulfilled or finalized orders.");
        }

        if (this.domainEvents.Count == 0)
        {
            throw new InvalidDomainStateException("Order must contain at least one domain event.");
        }
    }

    public void ConfirmPayment(DateTimeOffset confirmedAtUtc)
    {
        this.EnsureStatus(OrderStatus.Created, nameof(ConfirmPayment));

        this.Status = OrderStatus.Confirmed;

        this.AddDomainEvent(new OrderConfirmedDomainEvent { Id = this.Id, AddedAtUtc = confirmedAtUtc, });
    }

    public void Dispatch(DateTimeOffset dispatchedAtUtc)
    {
        this.EnsureStatus(OrderStatus.Confirmed, nameof(Dispatch));

        this.Status = OrderStatus.Fulfilled;

        this.AddDomainEvent(new OrderDispatchedDomainEvent { Id = this.Id, AddedAtUtc = dispatchedAtUtc, });
    }

    public void DeliverOrder(DateTimeOffset deliveredAtUtc)
    {
        this.EnsureStatus(OrderStatus.Fulfilled, nameof(DeliverOrder));

        this.Status = OrderStatus.Finalized;

        this.AddDomainEvent(new OrderDeliveredDomainEvent { Id = this.Id, AddedAtUtc = deliveredAtUtc, });
    }

    public void Cancel(DateTimeOffset cancelledAtUtc)
    {
        if (this.Status is OrderStatus.Fulfilled or OrderStatus.Finalized or OrderStatus.Cancelled)
        {
            throw new InvalidDomainStateException($"Cannot cancel order in status {this.Status}.");
        }

        var oldStatus = this.Status;
        this.Status = OrderStatus.Cancelled;

        this.AddDomainEvent(new OrderCancelledDomainEvent { Id = Id, OldStatus = oldStatus, AddedAtUtc = cancelledAtUtc, });
    }

    internal void ClearDomainEvents() => this.domainEvents.Clear();

    internal void MarkDomainEventAsPublished(DomainEvent domainEvent)
    {
        if (domainEvent is null)
        {
            throw new ArgumentNullException(nameof(domainEvent));
        }

        if (!this.domainEvents.Contains(domainEvent))
        {
            throw new InvalidDomainStateException("Cannot mark a domain event that does not belong to this aggregate.");
        }

        domainEvent.IsPublished = true;
    }

    private void AddDomainEvent(DomainEvent domainEvent, DateTimeOffset? addedAtUtc = null)
    {
        if (domainEvent is null)
        {
            throw new ArgumentNullException(nameof(domainEvent));
        }

        domainEvent.AddedAtUtc = addedAtUtc ?? DateTimeOffset.UtcNow;
        domainEvent.IsPublished = false;

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
