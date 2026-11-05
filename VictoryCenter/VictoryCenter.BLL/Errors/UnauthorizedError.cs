using FluentResults;

namespace VictoryCenter.BLL.Errors;

public class UnauthorizedError : Error
{
    public UnauthorizedError(string message)
        : base(message)
    {
    }
}
