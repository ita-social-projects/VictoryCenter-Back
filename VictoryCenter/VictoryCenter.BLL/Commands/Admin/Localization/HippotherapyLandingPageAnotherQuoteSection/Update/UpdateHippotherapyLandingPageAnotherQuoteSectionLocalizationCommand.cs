using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Update;

public record UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(
    UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>;
