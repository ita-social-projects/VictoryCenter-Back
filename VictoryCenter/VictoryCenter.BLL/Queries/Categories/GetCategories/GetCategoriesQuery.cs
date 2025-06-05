using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Queries.Categories.GetCategories;

public record GetCategoriesQuery()
    : IRequest<Result<IEnumerable<CategoryDto>>>;
