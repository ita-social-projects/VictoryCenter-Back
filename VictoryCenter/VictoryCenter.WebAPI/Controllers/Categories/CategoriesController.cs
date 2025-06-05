using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Categories.CreateCategory;
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
}
