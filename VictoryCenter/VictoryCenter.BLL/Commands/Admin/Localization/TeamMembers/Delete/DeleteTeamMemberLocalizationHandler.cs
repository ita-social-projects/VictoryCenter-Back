using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;

public class DeleteTeamMemberLocalizationHandler : IRequestHandler<DeleteTeamMemberLocalizationCommand, Result<DeleteTeamMemberLocalizationDto>>
{
    private readonly ILocalizationService<TeamMember, TeamMemberLocalization> _localizationService;

    public DeleteTeamMemberLocalizationHandler(ILocalizationService<TeamMember, TeamMemberLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeleteTeamMemberLocalizationDto>> Handle(DeleteTeamMemberLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeleteTeamMemberLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteTeamMemberLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMemberLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteTeamMemberLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamMemberLocalization)));
        }
    }
}
