using FluentResults;
using Microsoft.AspNetCore.Http;

namespace VictoryCenter.BLL.Errors;

public class ConcurrencyConflictError : Error
{
    public ConcurrencyConflictError(string message)
        : base(message)
    {
        Metadata.Add("StatusCode", StatusCodes.Status409Conflict);
    }
}
