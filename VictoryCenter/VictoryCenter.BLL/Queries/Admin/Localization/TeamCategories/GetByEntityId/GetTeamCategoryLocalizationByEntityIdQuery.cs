using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamCategories.GetByEntityId;
public record GetTeamCategoryLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<TeamCategoryLocalizationDto>>>;
