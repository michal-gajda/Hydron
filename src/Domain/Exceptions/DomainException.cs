namespace Hydron.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public abstract string ErrorCode { get; protected set; }

    protected DomainException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}
