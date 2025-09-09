using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Reorder;

public record ReorderTeamMembersCommand(ReorderTeamMembersDto ReorderTeamMembersDto)
    : IRequest<Result<Unit>>;
