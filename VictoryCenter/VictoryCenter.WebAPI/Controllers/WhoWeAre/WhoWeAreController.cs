using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.WhoWeAre.Update;
using VictoryCenter.BLL.DTOs.WhoWeAreContent;
using VictoryCenter.BLL.Queries.WhoWeAreSections.GetAll;
using VictoryCenter.BLL.Queries.WhoWeAreSections.GetByType;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.WebAPI.Controllers.WhoWeAre;

public class WhoWeAreController : BaseApiController
{
    [HttpPut("{sectionType}")]
    public async Task<IActionResult> UpdateWhoWeAreSection(List<CreateWhoWeAreContentDto> dtos, SectionType sectionType)
    {
        return HandleResult(await Mediator.Send(new UpdateWhoWeAreContentCommand(sectionType, dtos)));
    }

    [HttpGet("{sectionType}")]
    public async Task<IActionResult> GetWhoWeAreSection(SectionType sectionType)
    {
        return HandleResult(await Mediator.Send(new GetWhoWeAreSectionQuery(sectionType)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWhoWeAreSections()
    {
        return HandleResult(await Mediator.Send(new GetAllWhoWeAreSectionsQuery()));
    }
}
