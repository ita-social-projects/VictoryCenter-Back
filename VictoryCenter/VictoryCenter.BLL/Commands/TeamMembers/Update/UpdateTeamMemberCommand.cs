using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Commands.TeamMembers.Update;

public record UpdateTeamMemberCommand(UpdateTeamMemberDto updateTeamMemberDto, long id)
    : IRequest<Result<TeamMemberDto>>;
