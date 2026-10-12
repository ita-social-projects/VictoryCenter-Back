using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.Update;

public record UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand(
    UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>>;
