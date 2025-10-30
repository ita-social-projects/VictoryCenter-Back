using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.WhoWeAre.Update;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetByType;
using VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetPreviews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class WhoWeAreController : AuthorizedApiController
{
    [HttpPut("{sectionType}")]
    public async Task<IActionResult> UpdateWhoWeAreSection(List<UpdateWhoWeAreContentDto> dtos, SectionType sectionType)
    {
        return HandleResult(await Mediator.Send(new UpdateWhoWeAreContentCommand(sectionType, dtos)));
    }

    [HttpGet("{sectionType}")]
    public async Task<IActionResult> GetWhoWeAreSection(SectionType sectionType)
    {
        return HandleResult(await Mediator.Send(new GetWhoWeAreSectionQuery(sectionType)));
    }

    [HttpGet("previews")]
    public async Task<IActionResult> GetWhoWeAreSectionPreviews()
    {
        return HandleResult(await Mediator.Send(new GetWhoWeAreSectionPreviewsQuery()));
    }
}
