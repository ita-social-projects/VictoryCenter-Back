using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;
using VictoryCenter.BLL.Commands.Admin.Partners.Create;
using VictoryCenter.BLL.Commands.Admin.Partners.Delete;
using VictoryCenter.BLL.Commands.Admin.Partners.Reorder;
using VictoryCenter.BLL.Commands.Admin.Partners.Update;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Queries.Admin.Partners.GetAll;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class PartnersController : AuthController
{
    [HttpGet("pages")]
    [ProducesResponseType(typeof(List<PartnersSectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartnersSections()
    {
        return HandleResult(await Mediator.Send(new GetAllPartnersSectionsQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(FaqQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePartnersSection([FromBody] CreatePartnersSectionDto createPartnersSectionDto)
    {
               return HandleResult(await Mediator.Send(new CreatePartnersSectionCommand(createPartnersSectionDto)));
    }

    [HttpPut]
    [Route("{id:long}")]
    [ProducesResponseType(typeof(FaqQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePartnersSection([FromBody] UpdatePartnersSectionDto updatePartnersSectionDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdatePartnersSectionCommand(updatePartnersSectionDto, id)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartnersSection([FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new DeletePartnersSectionCommand(id)));
    }

    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderPartnerSections([FromBody] ReorderPartnersSectionsDto reorderPartnersSectionsDto)
    {
        return HandleResult(await Mediator.Send(new ReorderPartnersSectionsCommand(reorderPartnersSectionsDto)));
    }
}
