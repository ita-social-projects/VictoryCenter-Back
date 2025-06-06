using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMember;

namespace VictoryCenter.BLL.Queries.TeamMembers;

public record GetTeamMemberByIdQuery(long Id) 
    : IRequest<Result<TeamMemberDto>>;
