using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Create;

public record CreateTeamMemberCommand(CreateTeamMemberDto CreateTeamMemberDto)
    : IRequest<Result<TeamMemberDto>>;
