namespace Foxminded.HryvniaRateBot.App.Exceptions;

public abstract class BaseException : Exception
{
    public BaseException()
    {
    }

    public BaseException(string message)
        : base(message)
    {
    }

    public BaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
