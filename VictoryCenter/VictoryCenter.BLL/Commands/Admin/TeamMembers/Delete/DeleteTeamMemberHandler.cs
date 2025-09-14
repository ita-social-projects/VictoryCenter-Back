using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Services.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Delete;

public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;

    public DeleteTeamMemberHandler(IRepositoryWrapper repositoryWrapper, IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
    }

    /*    public async Task<Result<long>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var entityToDelete =
                await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
                {
                    Filter = entity => entity.Id == request.Id,
                    Include = query => query.Include(tm => tm.Category)
                });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
            }

            entityToDelete.Category = null!;

            _repositoryWrapper.TeamMembersRepository.Delete(entityToDelete);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(entityToDelete.Id);
            }

            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMember)));
        }*/

    public async Task<Result<long>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete = await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
        {
            Filter = entity => entity.Id == request.Id
        });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
        }

        await _reorderService.RemoveFromListAsync<TeamMember, long>(entityToDelete.Id, tm => tm.CategoryId == entityToDelete.CategoryId);

        _repositoryWrapper.TeamMembersRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMember)));
    }

}
