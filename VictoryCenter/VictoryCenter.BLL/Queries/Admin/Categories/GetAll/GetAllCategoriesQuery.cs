using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Categories;

namespace VictoryCenter.BLL.Queries.Admin.Categories.GetAll;

public record GetAllCategoriesQuery
    : IRequest<Result<IEnumerable<CategoryDto>>>;
