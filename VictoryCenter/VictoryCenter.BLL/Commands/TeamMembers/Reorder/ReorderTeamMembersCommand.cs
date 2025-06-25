using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Commands.TeamMembers.Reorder;

public record ReorderTeamMembersCommand(ReorderTeamMembersDto ReorderTeamMembersDto)
    : IRequest<Result<Unit>>;
