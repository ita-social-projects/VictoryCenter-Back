using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Commands.Categories.Update;

public record UpdateCategoryCommand(UpdateCategoryDto updateCategoryDto)
    : IRequest<Result<CategoryDto>>;
