namespace Hydron.Domain.ValueObjects;

public readonly record struct CustomerId
{
    public Guid Value { get; private init; }

    public CustomerId(Guid value)
    {
        Guard.AgainstDefault(value, "Customer ID cannot be an empty GUID.");
        this.Value = value;
    }

    public override string ToString() => this.Value.ToString();
}
