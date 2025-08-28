using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Queries.TeamMembers.GetByFilters;

public record GetTeamMembersByFiltersQuery(TeamMembersFilterDto TeamMembersFilter)
    : IRequest<Result<PaginationResult<TeamMemberDto>>>;
