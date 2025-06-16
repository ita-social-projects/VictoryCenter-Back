using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMember;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetByFilters;

public record GetTeamMembersByFiltersQuery(TeamMembersFilterDto TeamMembersFilter)
    : IRequest<Result<List<TeamMemberDto>>>;
