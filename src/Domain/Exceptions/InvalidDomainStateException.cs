namespace Hydron.Domain.Exceptions;

public sealed class InvalidDomainStateException : DomainException
{
    public override string ErrorCode { get; protected set; } = "invalid_domain_state";

    public InvalidDomainStateException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}
