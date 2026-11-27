using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Queries.Public.Localization.TeamMembers.GetByEntityId;

public record GetTeamMemberLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<TeamMemberLocalizationDto>>>;
