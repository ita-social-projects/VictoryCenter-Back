using FluentResults;
using MediatR;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.TeamMember.DeleteTeamMember;

public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteTeamMemberHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<Unit>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repositoryWrapper.TeamMemberRepository.ExistsAsync(request.Id);
        if (!exists)
        {
            return Result.Fail<Unit>($"Team member with ID {request.Id} not found.");
        }

        var deleted = await _repositoryWrapper.TeamMemberRepository.DeleteAsync(request.Id);
        if (!deleted)
        {
            return Result.Fail<Unit>("Failed to delete the team member.");
        }

        return Result.Ok(Unit.Value);
    }
}
