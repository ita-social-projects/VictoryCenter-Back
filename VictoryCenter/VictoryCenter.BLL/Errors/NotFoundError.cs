using FluentResults;

namespace VictoryCenter.BLL.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message)
        : base(message)
    {
    }
}
