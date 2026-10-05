using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Update;

public record UpdateHippotherapyLandingPageDescriptionSectionLocalizationCommand(
    UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageDescriptionSectionLocalizationDto>>;
