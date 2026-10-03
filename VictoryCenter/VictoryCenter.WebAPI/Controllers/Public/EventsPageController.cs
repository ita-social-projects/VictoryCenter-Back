using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Admin.EventsPage.Get;
using VictoryCenter.WebAPI.Controllers.Common;

using VictoryCenter.BLL.DTOs.Public.EventNews;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class EventsPageController : BaseApiController
{
    [HttpGet("public")]
    [ProducesResponseType(typeof(EventsIntroSectionPublicDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventsIntroSection()
    {
        var result = await Mediator.Send(new GetEventsIntroSectionQuery());
        if (result.IsFailed)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var publicDto = new EventsIntroSectionPublicDto
        {
            EventsBlockTitle = dto.IsEventsBlockTitleHidden ? string.Empty : dto.EventsBlockTitle,
            PageDescription = dto.IsPageDescriptionHidden ? string.Empty : dto.PageDescription
        };

        return Ok(publicDto);
    }
}
