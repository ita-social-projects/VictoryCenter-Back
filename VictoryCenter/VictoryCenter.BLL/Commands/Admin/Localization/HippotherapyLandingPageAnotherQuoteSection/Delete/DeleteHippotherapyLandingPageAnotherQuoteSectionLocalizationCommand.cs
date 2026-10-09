using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Delete;

public record DeleteHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>;
