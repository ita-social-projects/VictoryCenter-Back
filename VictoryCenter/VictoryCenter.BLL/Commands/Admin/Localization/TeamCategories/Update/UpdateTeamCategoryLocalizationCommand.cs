using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Update;
public record UpdateTeamCategoryLocalizationCommand(UpdateTeamCategoryLocalizationDto UpdateTeamCategoryLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<TeamCategoryLocalizationDto>>;
