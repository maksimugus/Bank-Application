namespace Lab5.Application.Exceptions;

public class NotAuthorizedException : Exception
{
    public NotAuthorizedException()
        : base("You are not authorized")
    {
    }

    public NotAuthorizedException(string? message)
        : base(message)
    {
    }

    public NotAuthorizedException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}