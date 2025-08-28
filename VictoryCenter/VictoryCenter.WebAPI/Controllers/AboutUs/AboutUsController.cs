using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.AboutUs.Update;
using VictoryCenter.BLL.DTOs.AboutUsContent;

namespace VictoryCenter.WebAPI.Controllers.AboutUs;

public class AboutUsController : BaseApiController
{
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAboutUsSection(List<CreateAboutUsContentDto> dtos, long id)
    {
        return HandleResult(await Mediator.Send(new AboutUsContentCommand(id, dtos)));
    }
}
