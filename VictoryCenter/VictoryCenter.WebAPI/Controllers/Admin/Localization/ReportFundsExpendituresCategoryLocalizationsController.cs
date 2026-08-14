using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Queries.Admin.Localization.ReportFundsExpendituresCategories.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class ReportFundsExpendituresCategoryLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<ReportFundsExpendituresCategoryLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportFundsExpendituresCategoryLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetReportFundsExpendituresCategoryLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReportFundsExpendituresCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateReportFundsExpendituresCategoryLocalization(
        [FromBody] CreateReportFundsExpendituresCategoryLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateReportFundsExpendituresCategoryLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(ReportFundsExpendituresCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReportFundsExpendituresCategoryLocalization(
        [FromBody] UpdateReportFundsExpendituresCategoryLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateReportFundsExpendituresCategoryLocalizationCommand(updateDto, EntityId, LanguageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteReportFundsExpendituresCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReportFundsExpendituresCategoryLocalization(
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteReportFundsExpendituresCategoryLocalizationCommand(EntityId, LanguageId)));
    }
}
