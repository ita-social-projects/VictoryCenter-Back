using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMemberLocalizations;

namespace VictoryCenter.BLL.Queries.Admin.TeamMemberLocalizations.GetByTeamMemberId;

public record GetByTeamMemberIdQuery(long Id)
    : IRequest<Result<IEnumerable<TeamMemberLocalizationDto>>>;
