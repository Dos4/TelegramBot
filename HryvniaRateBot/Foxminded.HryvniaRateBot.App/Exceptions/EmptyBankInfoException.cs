namespace Foxminded.HryvniaRateBot.App.Exceptions;

public class EmptyBankInfoException : BaseException
{
    public EmptyBankInfoException() : base()
    {
    }

    public EmptyBankInfoException(string message) : base(message)
    {
    }

    public EmptyBankInfoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
