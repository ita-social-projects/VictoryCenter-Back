using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Categories;

namespace VictoryCenter.BLL.Commands.Admin.Categories.Update;

public record UpdateCategoryCommand(UpdateCategoryDto updateCategoryDto)
    : IRequest<Result<CategoryDto>>;
