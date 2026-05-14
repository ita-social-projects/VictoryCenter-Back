using FluentResults;

namespace VictoryCenter.BLL.Errors;

public class InternalError : Error
{
    public InternalError(string message)
        : base(message)
    {
    }

    public InternalError(string message, Exception innerException)
        : base(message)
    {
        CausedBy(innerException);
    }
}
