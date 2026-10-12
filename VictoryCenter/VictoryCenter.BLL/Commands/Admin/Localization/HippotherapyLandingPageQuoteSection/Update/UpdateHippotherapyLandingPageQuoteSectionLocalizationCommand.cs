using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Update;

public record UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand(
    UpdateHippotherapyLandingPageQuoteSectionLocalizationDto UpdateHippotherapyLandingPageQuoteSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageQuoteSectionLocalizationDto>>;
