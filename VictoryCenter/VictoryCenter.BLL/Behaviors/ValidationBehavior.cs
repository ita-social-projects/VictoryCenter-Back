using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Behaviors.Abstractions;

namespace VictoryCenter.BLL.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseValidatableRequest
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (validationFailures.Count > 0)
        {
            throw new ValidationException(validationFailures);
        }

        return await next(cancellationToken);
    }
}
