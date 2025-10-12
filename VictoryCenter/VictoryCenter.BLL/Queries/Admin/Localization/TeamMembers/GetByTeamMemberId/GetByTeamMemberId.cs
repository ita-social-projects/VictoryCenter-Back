using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByTeamMemberId;

public record GetByTeamMemberIdQuery(long Id)
    : IRequest<Result<IEnumerable<TeamMemberLocalizationDto>>>;
