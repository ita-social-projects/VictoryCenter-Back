using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReorderService;
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

    public async Task<Result<long>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entityToDelete = await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
            {
                Filter = entity => entity.Id == request.Id
            });

            if (entityToDelete is null)
            {
                return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
            }

            using (var transactionScope = _repositoryWrapper.BeginTransaction())
            {
                var categoryId = entityToDelete.CategoryId;

                _repositoryWrapper.TeamMembersRepository.Delete(entityToDelete);

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMember)));
                }

                await _reorderService.RenumberPriorityAsync<TeamMember>(
                    groupSelector: tm => tm.CategoryId == categoryId);

                await _repositoryWrapper.SaveChangesAsync();

                transactionScope.Complete();

                return Result.Ok(entityToDelete.Id);
            }
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMember)));
        }
    }
}
