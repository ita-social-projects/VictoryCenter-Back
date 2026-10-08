using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageQuoteSection.GetByEntityId;

public record GetHippotherapyLandingPageQuoteSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageQuoteSectionLocalizationDto>>>;
