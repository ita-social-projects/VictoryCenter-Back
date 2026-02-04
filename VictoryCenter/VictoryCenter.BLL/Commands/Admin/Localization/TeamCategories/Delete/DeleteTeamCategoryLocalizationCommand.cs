using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Delete;
public record DeleteTeamCategoryLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteTeamCategoryLocalizationDto>>;
