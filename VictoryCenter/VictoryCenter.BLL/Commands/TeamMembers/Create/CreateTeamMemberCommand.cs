using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Commands.TeamMembers.Create;

public record CreateTeamMemberCommand(CreateTeamMemberDto createTeamMemberDto)
    : IRequest<Result<TeamMemberDto>>;
