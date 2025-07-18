using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Delete;

public record DeleteTeamMemberCommand(long Id) : IRequest<Result<long>>;
