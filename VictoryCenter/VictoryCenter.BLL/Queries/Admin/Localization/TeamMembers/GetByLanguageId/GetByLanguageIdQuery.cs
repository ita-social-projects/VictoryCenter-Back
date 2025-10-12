using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByLanguageId;

public record GetByLanguageIdQuery(long Id)
    : IRequest<Result<IEnumerable<TeamMemberLocalizationDto>>>;
