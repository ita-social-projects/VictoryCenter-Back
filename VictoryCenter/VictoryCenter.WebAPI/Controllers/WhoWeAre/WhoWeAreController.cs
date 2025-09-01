using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.WhoWeAre.Update;
using VictoryCenter.BLL.DTOs.WhoWeAreContent;
using VictoryCenter.BLL.Queries.WhoWeAreSections.GetAll;
using VictoryCenter.BLL.Queries.WhoWeAreSections.GetByType;

namespace VictoryCenter.WebAPI.Controllers.WhoWeAre;

public class WhoWeAreController : BaseApiController
{
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateWhoWeAreSection(List<CreateWhoWeAreContentDto> dtos, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateWhoWeAreContentCommand(id, dtos)));
    }

    [HttpGet("{sectionType}")]
    public async Task<IActionResult> GetWhoWeAreSection(string sectionType)
    {
        return HandleResult(await Mediator.Send(new GetWhoWeAreSectionQuery(sectionType)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWhoWeAreSections()
    {
        return HandleResult(await Mediator.Send(new GetAllWhoWeAreSectionsQuery()));
    }
}
