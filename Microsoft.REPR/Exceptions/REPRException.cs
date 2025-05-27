namespace Microsoft.REPR.Exceptions;

public sealed class REPRException : Exception
{
    public REPRException()
    {
    }

    public REPRException(string? message) : base(message)
    {
    }

    public REPRException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
