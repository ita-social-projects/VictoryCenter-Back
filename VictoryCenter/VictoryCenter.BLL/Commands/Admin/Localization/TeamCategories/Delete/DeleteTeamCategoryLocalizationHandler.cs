using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Delete;
public class DeleteTeamCategoryLocalizationHandler : IRequestHandler<DeleteTeamCategoryLocalizationCommand, Result<DeleteTeamCategoryLocalizationDto>>
{
    private readonly ILocalizationService<TeamCategory, TeamCategoryLocalization> _localizationService;

    public DeleteTeamCategoryLocalizationHandler(ILocalizationService<TeamCategory, TeamCategoryLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteTeamCategoryLocalizationDto>> Handle(DeleteTeamCategoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteTeamCategoryLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteTeamCategoryLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteTeamCategoryLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamCategoryLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteTeamCategoryLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamCategoryLocalization)));
        }
    }
}
