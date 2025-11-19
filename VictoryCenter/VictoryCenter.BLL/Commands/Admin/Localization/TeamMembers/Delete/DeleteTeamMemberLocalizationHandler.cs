using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;

public class DeleteTeamMemberLocalizationHandler : IRequestHandler<DeleteTeamMemberLocalizationCommand, Result<(long EntityId, long LanguageId)>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteTeamMemberLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<(long EntityId, long LanguageId)>> Handle(DeleteTeamMemberLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            TeamMemberLocalization? entityToDelete = await _repositoryWrapper.TeamMemberLocalizationsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<TeamMemberLocalization>
            {
                Filter = localization => localization.EntityId == request.EntityId &&
                                           localization.LanguageId == request.LanguageId
            });

            if (entityToDelete is null)
            {
                return Result.Fail<(long EntityId, long LanguageId)>(ErrorMessagesConstants
                    .NotFound((request.EntityId, request.LanguageId), typeof(TeamMemberLocalization)));
            }

            _repositoryWrapper.TeamMemberLocalizationsRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok((request.EntityId, request.LanguageId));
            }

            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMemberLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<(long EntityId, long LanguageId)>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamMemberLocalization)));
        }
    }
}
