using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;

public record GetTeamMembersByFiltersQuery(TeamMembersFilterDto TeamMembersFilter)
    : IRequest<Result<List<TeamMemberDto>>>;
