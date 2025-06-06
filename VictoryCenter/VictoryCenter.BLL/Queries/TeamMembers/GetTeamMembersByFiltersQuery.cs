using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMember;

namespace VictoryCenter.BLL.Queries.TeamMembers;

public record GetTeamMembersByFiltersQuery(TeamMembersFilterDto TeamMembersFilter)
    : IRequest<Result<List<TeamMemberDto>>>;
