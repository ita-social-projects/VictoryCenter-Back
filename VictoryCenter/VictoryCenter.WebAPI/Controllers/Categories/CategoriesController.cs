using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Categories.CreateCategory;
using VictoryCenter.BLL.Commands.Categories.DeleteCategory;
using VictoryCenter.BLL.Commands.Categories.UpdateCategory;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.Queries.Categories.GetCategories;

namespace VictoryCenter.WebAPI.Controllers.Categories;

public class CategoriesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        return HandleResult(await Mediator.Send(new GetCategoriesQuery()));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateCategoryCommand(createCategoryDto)));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto updateCategoryDto)
    {
        return HandleResult(await Mediator.Send(new UpdateCategoryCommand(updateCategoryDto)));
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        return HandleResult(await Mediator.Send(new DeleteCategoryCommand(id)));
    }
}
