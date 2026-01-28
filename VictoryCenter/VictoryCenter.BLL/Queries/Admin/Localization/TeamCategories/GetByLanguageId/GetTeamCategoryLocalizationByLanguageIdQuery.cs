using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamCategories.GetByLanguageId;
public record GetTeamCategoryLocalizationByLanguageIdQuery(long Id)
    : IRequest<Result<List<TeamCategoryLocalizationDto>>>;
