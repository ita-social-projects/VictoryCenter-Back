using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.TeamMembers.Delete;

public record DeleteTeamMemberCommand(long Id) : IRequest<Result<long>>;
