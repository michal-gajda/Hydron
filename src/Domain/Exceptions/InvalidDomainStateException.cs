namespace Hydron.Domain.Exceptions;

public sealed class InvalidDomainStateException : Exception
{
    public InvalidDomainStateException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}
