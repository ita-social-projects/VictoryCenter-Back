using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;

namespace VictoryCenter.BLL.Queries.Admin.TeamCategories.GetAll;

public record GetAllTeamCategoriesQuery
    : IRequest<Result<IEnumerable<TeamCategoryDto>>>;
