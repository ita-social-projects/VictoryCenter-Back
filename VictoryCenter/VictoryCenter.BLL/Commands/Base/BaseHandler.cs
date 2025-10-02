using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.BLL.Commands.Base;

public abstract class BaseHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await HandleRequest(request, cancellationToken);
            return Result.Ok(response);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<TResponse>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<TResponse>(ex.Message);
        }
    }

    public abstract Task<TResponse> HandleRequest(TRequest request, CancellationToken cancellationToken);
}
