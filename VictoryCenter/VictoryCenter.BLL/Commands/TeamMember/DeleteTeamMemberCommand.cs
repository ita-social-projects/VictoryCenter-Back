using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.TeamMember.DeleteTeamMember;

public record DeleteTeamMemberCommand(long Id) : IRequest<Result<Unit>>;
