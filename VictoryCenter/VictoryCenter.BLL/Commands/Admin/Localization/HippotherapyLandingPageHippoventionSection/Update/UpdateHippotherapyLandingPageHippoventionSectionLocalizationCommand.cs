using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionSection.Update;

public record UpdateHippotherapyLandingPageHippoventionSectionLocalizationCommand(
    UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageHippoventionSectionLocalizationDto>>;
