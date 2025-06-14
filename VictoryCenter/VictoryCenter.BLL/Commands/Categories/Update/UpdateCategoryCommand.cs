using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Commands.Categories.UpdateCategory;

public record UpdateCategoryCommand(UpdateCategoryDto updateCategoryDto)
    : IRequest<Result<CategoryDto>>;
