using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetById;

public record GetTeamMemberByIdQuery(long Id)
    : IRequest<Result<TeamMemberDto>>;
