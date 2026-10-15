using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Update;

public record UpdateHippotherapyLandingPageEthicsSectionLocalizationCommand(
    UpdateHippotherapyLandingPageEthicsSectionLocalizationDto UpdateHippotherapyLandingPageEthicsSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageEthicsSectionLocalizationDto>>;
