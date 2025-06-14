using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(CreateCategoryDto createCategoryDto)
    : IRequest<Result<CategoryDto>>;
