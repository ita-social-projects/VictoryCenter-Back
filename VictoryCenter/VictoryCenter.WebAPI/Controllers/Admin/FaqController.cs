using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.FaqQuestions.Create;
using VictoryCenter.BLL.Commands.FaqQuestions.Delete;
using VictoryCenter.BLL.Commands.FaqQuestions.Reorder;
using VictoryCenter.BLL.Commands.FaqQuestions.Update;
using VictoryCenter.BLL.DTOs.FaqQuestions;
using VictoryCenter.BLL.Queries.FaqQuestions.GetByFilters;
using VictoryCenter.BLL.Queries.FaqQuestions.GetById;
using VictoryCenter.BLL.Queries.Pages.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class FaqController : AuthorizedApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPages()
    {
        return HandleResult(await Mediator.Send(new GetAllPagesQuery()));
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
    public async Task<IActionResult> ReorderQuestions([FromBody] ReorderFaqQuestionsDto dto)
    {
        return HandleResult(await Mediator.Send(new ReorderFaqQuestionsCommand(dto)));
    }
}
