using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ReorderMetrics;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ToggleMetricVisibility;
using VictoryCenter.BLL.Commands.Admin.MainPage.Create;
using VictoryCenter.BLL.Commands.Admin.MainPage.Update;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class MainPageController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(MainPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMainPage([FromBody] CreateMainPageDto createMainPageDto)
    {
        return HandleResult(await Mediator.Send(new CreateMainPageCommand(createMainPageDto)));
    }

    [HttpPut]
    [ProducesResponseType(typeof(MainPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMainPage([FromBody] UpdateMainPageDto updateMainPageDto)
    {
        return HandleResult(await Mediator.Send(new UpdateMainPageCommand(updateMainPageDto)));
    }

    [HttpPut("metrics/{id:long}/visibility")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleMetricVisibility(
        [FromRoute] long id,
        [FromBody] UpdateMetricVisibilityDto dto)
    {
        return HandleResult(await Mediator.Send(new ToggleMetricVisibilityCommand(id, dto)));
    }

    [HttpPut("metrics/reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderMetrics([FromBody] ReorderMetricsDto reorderMetricsDto)
    {
        return HandleResult(await Mediator.Send(new ReorderMetricsCommand(reorderMetricsDto)));
    }
}
