using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Categories;

namespace VictoryCenter.BLL.Commands.Admin.Categories.Create;

public record CreateCategoryCommand(CreateCategoryDto createCategoryDto)
    : IRequest<Result<CategoryDto>>;
