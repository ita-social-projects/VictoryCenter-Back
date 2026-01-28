using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Create;
public record CreateTeamCategoryLocalizationCommand(CreateTeamCategoryLocalizationDto CreateTeamCategoryLocalizationDto)
    : IRequest<Result<TeamCategoryLocalizationDto>>;
