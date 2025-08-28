using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.Queries.TeamMembers.Search;

public record SearchTeamMemberQuery(SearchTeamMemberDto SearchTeamMemberDto)
    : IRequest<Result<PaginationResult<TeamMemberDto>>>;
