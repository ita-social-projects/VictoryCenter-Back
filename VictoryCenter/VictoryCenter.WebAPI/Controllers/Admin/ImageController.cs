using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Images.Create;
using VictoryCenter.BLL.Commands.Admin.Images.Delete;
using VictoryCenter.BLL.Commands.Admin.Images.Update;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.Queries.Admin.Images.GetById;
using VictoryCenter.BLL.Queries.Admin.Images.GetByName;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

[Authorize]
public class ImageController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult> CreateImage([FromBody] CreateImageDto request)
    {
        return HandleResult(await Mediator.Send(new CreateImageCommand(request)));
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult> GetImage(string name)
    {
        return HandleResult(await Mediator.Send(new GetImageByNameQuery(name)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetImage(long id)
    {
        return HandleResult(await Mediator.Send(new GetImageByIdQuery(id)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateImage(long id, [FromBody] UpdateImageDto request)
    {
        return HandleResult(await Mediator.Send(new UpdateImageCommand(request, id)));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteImage(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteImageCommand(id)));
    }
}
