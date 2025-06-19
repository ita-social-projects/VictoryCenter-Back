using FluentResults;
using MediatR;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.TeamMembers.Delete;

public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteTeamMemberHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var entityToDelete =
            await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
            {
                Filter = entity => entity.Id == request.Id
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>($"Team member with ID {request.Id} not found.");
        }

        entityToDelete.Category = null!; // Clear the reference to the Category to avoid foreign key constraint issues

        _repositoryWrapper.TeamMembersRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail<long>("Failed to delete team member.");
    }
}
