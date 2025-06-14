using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VictoryCenter.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??=
        HttpContext.RequestServices.GetService<IMediator>() !;
    
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.HasError(error => error.Message == "Unauthorized"))
        {
            return Unauthorized();
        }

        if (result.IsSuccess && result.ValueOrDefault is not null)
        {
            return Ok(result.Value);
        }

        if (!result.IsSuccess && result.ValueOrDefault is null)
        {
            return NotFound("Not Found");
        }

        return BadRequest(result.Errors);
    }
}
