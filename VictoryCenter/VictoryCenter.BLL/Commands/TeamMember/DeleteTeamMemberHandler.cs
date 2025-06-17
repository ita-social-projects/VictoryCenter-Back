using FluentResults;
using MediatR;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.TeamMember.DeleteTeamMember
{
    public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public DeleteTeamMemberHandler(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<Unit>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var teamMember = await _repositoryWrapper.TeamMemberRepository
                .GetFirstOrDefaultAsync(tm => tm.Id == request.Id);

            if (teamMember is null)
            {
                return Result.Fail<Unit>($"Team member with ID {request.Id} not found.");
            }

            _repositoryWrapper.TeamMemberRepository.Delete(teamMember);
            return Result.Ok(Unit.Value);
        }
    }
}