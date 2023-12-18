namespace Application.Exceptions;

public class InvalidValueException : Exception
{
    public InvalidValueException()
    {
    }

    public InvalidValueException(string? message)
        : base(message)
    {
    }

    public InvalidValueException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public InvalidValueException(string paramName, string value)
        : base($"Invalid value for {paramName}: {value}")
    {
    }
}