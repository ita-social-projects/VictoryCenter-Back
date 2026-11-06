using Microsoft.AspNetCore.Mvc;
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
            return BadRequest();
        }

        return Ok(_contentLockManager.GetStatus(resource));
    }
}
