using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Create;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresCategories.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ReportFundsExpendituresCategoriesController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportFundsExpendituresCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportFundsExpendituresCategories()
    {
        return HandleResult(await Mediator.Send(new GetAllReportFundsExpendituresCategoriesQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReportFundsExpendituresCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReportFundsExpendituresCategory(
        CreateReportFundsExpendituresCategoryDto createReportFundsExpendituresCategoryDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateReportFundsExpendituresCategoryCommand(createReportFundsExpendituresCategoryDto)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ReportFundsExpendituresCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReportFundsExpendituresCategory(
        UpdateReportFundsExpendituresCategoryDto updateReportFundsExpendituresCategoryDto, long id)
    {
        return HandleResult(await Mediator.Send(
            new UpdateReportFundsExpendituresCategoryCommand(updateReportFundsExpendituresCategoryDto, id)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReportFundsExpendituresCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteReportFundsExpendituresCategoryCommand(id)));
    }
}
