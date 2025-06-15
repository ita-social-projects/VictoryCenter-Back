using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Commands.Categories.Create;

public record CreateCategoryCommand(CreateCategoryDto createCategoryDto)
    : IRequest<Result<CategoryDto>>;
