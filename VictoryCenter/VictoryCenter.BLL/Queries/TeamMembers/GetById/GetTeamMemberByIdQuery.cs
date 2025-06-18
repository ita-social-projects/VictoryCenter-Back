using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetById;

public record GetTeamMemberByIdQuery(long Id)
    : IRequest<Result<TeamMemberDto>>;
