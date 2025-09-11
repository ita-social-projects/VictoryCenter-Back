using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamMemberLocalizations;

namespace VictoryCenter.BLL.Queries.Admin.TeamMemberLocalizations.GetByLanguageId;

public record GetByLanguageIdQuery(long Id)
    : IRequest<Result<IEnumerable<TeamMemberLocalizationDto>>>;
