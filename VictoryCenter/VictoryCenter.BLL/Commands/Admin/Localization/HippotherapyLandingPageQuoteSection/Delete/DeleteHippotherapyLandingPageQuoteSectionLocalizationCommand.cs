using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Delete;

public record DeleteHippotherapyLandingPageQuoteSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyLandingPageQuoteSectionLocalizationDto>>;
