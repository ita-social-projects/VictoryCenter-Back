using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.GetByEntityId;

public record GetHippotherapyLandingPageAnotherQuoteSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>>;
