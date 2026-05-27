namespace Hydron.Domain.ValueObjects;

public readonly record struct OrderId
{
    public Guid Value { get; private init; }

    public OrderId(Guid value)
    {
        Guard.AgainstDefault(value, "Order ID cannot be an empty GUID.");
        this.Value = value;
    }

    public override string ToString() => this.Value.ToString();
}
