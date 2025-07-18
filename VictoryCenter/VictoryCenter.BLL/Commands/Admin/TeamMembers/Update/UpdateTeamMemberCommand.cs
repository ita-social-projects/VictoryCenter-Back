using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Update;

public record UpdateTeamMemberCommand(UpdateTeamMemberDto updateTeamMemberDto, long id)
    : IRequest<Result<TeamMemberDto>>;
