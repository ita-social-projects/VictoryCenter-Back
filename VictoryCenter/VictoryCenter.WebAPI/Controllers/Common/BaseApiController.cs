using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VictoryCenter.BLL.Errors;

namespace VictoryCenter.WebAPI.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??=
        HttpContext.RequestServices.GetService<IMediator>()!;

    protected ProblemDetails CreateProblemDetails(int statusCode, string? detail = null)
    {
        var problemsFactory = HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        return problemsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: statusCode,
            detail: detail);
    }

    // Field-grouped shape (errors: { field: [messages] }) for validation failures. Errors that carry a
    // FluentValidation property name (see ValidationError) are grouped under that field; any other error
    // mixed into the same Result is grouped under the empty key, so it is still represented in the response
    // instead of being silently dropped. Mirrors the shape the framework already produces for
    // model-binding/data-annotation failures via CreateValidationProblemDetails.
    protected ValidationProblemDetails CreateValidationProblemDetails(IEnumerable<IError> errors)
    {
        var problemsFactory = HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        var modelState = new ModelStateDictionary();
        foreach (var error in errors)
        {
            var key = error is ValidationError validationError ? validationError.PropertyName : string.Empty;
            modelState.AddModelError(key, error.Message);
        }

        return problemsFactory.CreateValidationProblemDetails(
            HttpContext,
            modelState,
            statusCode: StatusCodes.Status400BadRequest);
    }

    protected ActionResult HandleResult<T>(Result<T> result, string? unauthorizedDetail = null)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        // Typed checks come first; the string-matching fallbacks below exist only because
        // not every "not found"/"unauthorized" Result.Fail(...) call site has been migrated
        // to the typed errors yet.
        if (result.HasError<NotFoundError>() || result.HasError(error
            => error.Message.Contains("not found", StringComparison.CurrentCultureIgnoreCase)))
        {
            return NotFound(CreateProblemDetails(StatusCodes.Status404NotFound));
        }

        if (result.HasError<UnauthorizedError>() || result.HasError(error => error.Message == "Unauthorized"))
        {
            return Unauthorized(CreateProblemDetails(StatusCodes.Status401Unauthorized, unauthorizedDetail));
        }

        if (result.HasError<InternalError>())
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                CreateProblemDetails(StatusCodes.Status500InternalServerError));
        }

        if (result.HasError<ConcurrencyConflictError>())
        {
            var conflictDetail = string.Join("; ", result.Errors.OfType<ConcurrencyConflictError>().Select(e => e.Message));
            return Conflict(CreateProblemDetails(StatusCodes.Status409Conflict, conflictDetail));
        }

        if (result.HasError<ValidationError>())
        {
            return BadRequest(CreateValidationProblemDetails(result.Errors));
        }

        var errorDetail = string.Join("; ", result.Errors.Select(e => e.Message));
        return BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, errorDetail));
    }
}
