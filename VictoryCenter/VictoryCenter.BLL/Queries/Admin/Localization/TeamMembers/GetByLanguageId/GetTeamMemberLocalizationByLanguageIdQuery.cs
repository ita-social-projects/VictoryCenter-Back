using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByLanguageId;

public record GetTeamMemberLocalizationByLanguageIdQuery(long Id)
    : IRequest<Result<List<TeamMemberLocalizationDto>>>;
