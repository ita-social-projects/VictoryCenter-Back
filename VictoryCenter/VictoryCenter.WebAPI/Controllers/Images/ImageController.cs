using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Images.Create;
using VictoryCenter.BLL.Commands.Images.Update;
using VictoryCenter.BLL.Commands.Images.Delete;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Queries.Images.GetById;
using VictoryCenter.BLL.Queries.Images.GetByName;

namespace VictoryCenter.WebAPI.Controllers.Images;

public class ImageController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult> CreateImage([FromBody] CreateImageDTO request)
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
    public async Task<ActionResult> UpdateImage(long id, [FromBody] UpdateImageDTO request)
    {
        return HandleResult(await Mediator.Send(new UpdateImageCommand(request)));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteImage(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteImageCommand(id)));
    }
}
