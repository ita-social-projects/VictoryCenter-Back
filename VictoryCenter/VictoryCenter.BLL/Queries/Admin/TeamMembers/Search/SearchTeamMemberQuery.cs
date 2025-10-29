using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.TeamMembers.Search;

public record SearchTeamMemberQuery(SearchTeamMemberDto SearchTeamMemberDto)
    : IRequest<Result<PaginationResult<TeamMemberDto>>>;
