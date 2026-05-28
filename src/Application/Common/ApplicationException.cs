namespace Hydron.Application.Common;

public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}
