using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.TeamMembers;

public record DeleteTeamMemberCommand(long Id) : IRequest<Result<long>>;
