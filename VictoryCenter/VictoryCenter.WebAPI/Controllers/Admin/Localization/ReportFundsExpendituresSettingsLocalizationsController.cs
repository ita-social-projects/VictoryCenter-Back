using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Queries.Admin.Localization.ReportFundsExpendituresSettings.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class ReportFundsExpendituresSettingsLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<ReportFundsExpendituresSettingsLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportFundsExpendituresSettingsLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetReportFundsExpendituresSettingsLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReportFundsExpendituresSettingsLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateReportFundsExpendituresSettingsLocalization(
        [FromBody] CreateReportFundsExpendituresSettingsLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateReportFundsExpendituresSettingsLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(ReportFundsExpendituresSettingsLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReportFundsExpendituresSettingsLocalization(
        [FromBody] UpdateReportFundsExpendituresSettingsLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateReportFundsExpendituresSettingsLocalizationCommand(updateDto, EntityId, LanguageId)));
    }
}
