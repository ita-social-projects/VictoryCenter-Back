using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;

namespace VictoryCenter.BLL.Queries.Admin.Localization.WhoWeAreContents.GetByLanguageId;

public record GetWhoWeAreContentLocalizationByLanguageIdQuery(long Id)
    : IRequest<Result<List<WhoWeAreContentLocalizationDto>>>;
