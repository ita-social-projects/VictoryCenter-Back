using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Commands.TeamMembers.CreateTeamMember;

public record CreateTeamMemberCommand(CreateTeamMemberDto createTeamMemberDto)
    : IRequest<Result<TeamMemberDto>>;
