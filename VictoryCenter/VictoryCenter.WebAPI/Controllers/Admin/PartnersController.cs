using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Partners.Create;
using VictoryCenter.BLL.Commands.Admin.Partners.Delete;
using VictoryCenter.BLL.Commands.Admin.Partners.ReorderPartners;
using VictoryCenter.BLL.Commands.Admin.Partners.ReorderSections;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateSection;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Queries.Admin.Partners.GetBanner;
using VictoryCenter.BLL.Queries.Admin.Partners.GetPartnerSections;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class PartnersController : AuthorizedApiController
{
    [HttpGet("sections")]
    [ProducesResponseType(typeof(List<PartnersSectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartnersSections()
    {
        return HandleResult(await Mediator.Send(new GetPartnerSectionsQuery()));
    }

    [HttpGet("banner")]
    [ProducesResponseType(typeof(PartnersPageBannerDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartnersPageBanner()
    {
        return HandleResult(await Mediator.Send(new GetPartnersPageBannerQuery()));
    }

    [HttpPut("banner")]
    [ProducesResponseType(typeof(PartnersPageBannerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartnersPageBanner([FromBody] UpdatePartnersPageBannerDto updatePartnersPageBannerDto)
    {
        return HandleResult(await Mediator.Send(new UpdatePartnersPageBannerCommand(updatePartnersPageBannerDto)));
    }

    [HttpPost("sections")]
    [ProducesResponseType(typeof(PartnersSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePartnersSection([FromBody] CreatePartnersSectionDto createPartnersSectionDto)
    {
        return HandleResult(await Mediator.Send(new CreatePartnersSectionCommand(createPartnersSectionDto)));
    }

    [HttpPut]
    [Route("sections/{id:long}")]
    [ProducesResponseType(typeof(PartnersSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePartnersSection([FromBody] UpdatePartnersSectionDto updatePartnersSectionDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdatePartnersSectionCommand(updatePartnersSectionDto, id)));
    }

    [HttpDelete]
    [Route("sections/{id:long}")]
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
    public async Task<IActionResult> ReorderPartners([FromBody] ReorderPartnersDto reorderPartnersDto)
    {
        return HandleResult(await Mediator.Send(new ReorderPartnersCommand(reorderPartnersDto)));
    }

    [HttpPut("sections/reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderPartnersSections([FromBody] ReorderPartnersSectionsDto reorderPartnersSectionsDto)
    {
        return HandleResult(await Mediator.Send(new ReorderPartnersSectionsCommand(reorderPartnersSectionsDto)));
    }
}
