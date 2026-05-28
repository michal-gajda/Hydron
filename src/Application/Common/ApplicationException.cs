namespace Hydron.Application.Common;

public abstract class ApplicationException : Exception
{
    public abstract string ErrorCode { get; protected set; }

    protected ApplicationException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}
