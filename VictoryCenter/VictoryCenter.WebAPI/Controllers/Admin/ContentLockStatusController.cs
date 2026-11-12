using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

[Route("api/content-locks")]
public sealed class ContentLockStatusController : AuthorizedApiController
{
    private readonly IContentLockManager _contentLockManager;
    private readonly IContentLockResourcePolicy _resourcePolicy;

    public ContentLockStatusController(
        IContentLockManager contentLockManager,
        IContentLockResourcePolicy resourcePolicy)
    {
        _contentLockManager = contentLockManager;
        _resourcePolicy = resourcePolicy;
    }

    [HttpGet("{resourceType}/{resourceId:long}")]
    [ProducesResponseType(typeof(ContentLockStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult GetStatus(string resourceType, long resourceId)
    {
        var resource = new ContentLockResourceDto
        {
            ResourceType = resourceType,
            ResourceId = resourceId,
        };
        if (!_resourcePolicy.IsSupported(resource))
        {
            var problemDetailsFactory = HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            return BadRequest(problemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: StatusCodes.Status400BadRequest,
                detail: ContentLockConstants.UnsupportedResourceMessage));
        }

        return Ok(_contentLockManager.GetStatus(resource));
    }
}
