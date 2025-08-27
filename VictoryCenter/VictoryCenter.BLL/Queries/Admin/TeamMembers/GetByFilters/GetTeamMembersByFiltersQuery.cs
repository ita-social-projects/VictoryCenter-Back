using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.GetByFilters;

public record GetTeamMembersByFiltersQuery(TeamMembersFilterDto TeamMembersFilter)
    : IRequest<Result<PaginationResult<TeamMemberDto>>>;
