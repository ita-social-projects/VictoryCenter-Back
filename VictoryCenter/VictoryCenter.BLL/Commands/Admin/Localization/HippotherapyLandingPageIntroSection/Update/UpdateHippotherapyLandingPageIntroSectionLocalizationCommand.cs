using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Update;

public record UpdateHippotherapyLandingPageIntroSectionLocalizationCommand(
    UpdateHippotherapyLandingPageIntroSectionLocalizationDto UpdateHippotherapyLandingPageIntroSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageIntroSectionLocalizationDto>>;
