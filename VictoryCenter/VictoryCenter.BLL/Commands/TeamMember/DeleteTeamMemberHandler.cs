using MediatR;
using VictoryCenter.DAL.Repositories.Interfaces;
using VictoryCenter.Shared;

namespace VictoryCenter.BLL.Commands.TeamMember.DeleteTeamMember;

public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<Unit>>
{
    private readonly ITeamMemberRepository _teamMemberRepository;

    public DeleteTeamMemberHandler(ITeamMemberRepository teamMemberRepository)
    {
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<Result<Unit>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var exists = await _teamMemberRepository.ExistsAsync(request.Id);
        if (!exists)
        {
            return Result<Unit>.Failure($"Team member with ID {request.Id} not found.");
        }

        var deleted = await _teamMemberRepository.DeleteAsync(request.Id);
        if (!deleted)
        {
            return Result<Unit>.Failure("Failed to delete the team member.");
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
