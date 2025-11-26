using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByEntityId;

public record GetTeamMemberLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<IEnumerable<TeamMemberLocalizationDto>>>;
