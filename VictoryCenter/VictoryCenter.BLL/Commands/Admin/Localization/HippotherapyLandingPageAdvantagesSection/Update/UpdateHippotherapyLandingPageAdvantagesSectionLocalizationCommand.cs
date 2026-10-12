using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Update;

public record UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(
    UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>;
