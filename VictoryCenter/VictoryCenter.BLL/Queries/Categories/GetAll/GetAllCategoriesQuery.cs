using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.Queries.Categories.GetAll;

public record GetAllCategoriesQuery
    : IRequest<Result<IEnumerable<CategoryDto>>>;
