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
        if (result.IsSuccess && result.ValueOrDefault is not null)
        {
            return Ok(result.Value);
        }

        if (result.ValueOrDefault is null)
        {
            return NotFound("Not Found");
        }

        if (result.HasError(error => error.Message == "Unauthorized"))
        {
            return Unauthorized();
        }

        return BadRequest(result.Errors);
    }
}
