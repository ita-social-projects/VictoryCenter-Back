using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Create;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Delete;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Update;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetById;
using VictoryCenter.BLL.Queries.Admin.VisitorPages.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class FaqController : AuthorizedApiController
{
    [HttpGet("pages")]
    public async Task<IActionResult> GetAllPages()
    {
        return HandleResult(await Mediator.Send(new GetAllVisitorPagesQuery()));
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredQuestions([FromQuery] FaqQuestionsFilterDto faqQuestionsFilterDto)
    {
        return HandleResult(await Mediator.Send(new GetFaqQuestionsByFiltersQuery(faqQuestionsFilterDto)));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetQuestionById([FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new GetFaqQuestionByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateFaqQuestionDto createFaqQuestionDto)
    {
        return HandleResult(await Mediator.Send(new CreateFaqQuestionCommand(createFaqQuestionDto)));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteQuestion([FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new DeleteFaqQuestionCommand(id)));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateQuestion(
        [FromBody] UpdateFaqQuestionDto updateFaqQuestionDto,
        [FromRoute] long id)
    {
        return HandleResult(await Mediator.Send(new UpdateFaqQuestionCommand(updateFaqQuestionDto, id)));
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderQuestions([FromBody] ReorderFaqQuestionsDto reorderFaqQuestionsDto)
    {
        return HandleResult(await Mediator.Send(new ReorderFaqQuestionsCommand(reorderFaqQuestionsDto)));
    }
}
