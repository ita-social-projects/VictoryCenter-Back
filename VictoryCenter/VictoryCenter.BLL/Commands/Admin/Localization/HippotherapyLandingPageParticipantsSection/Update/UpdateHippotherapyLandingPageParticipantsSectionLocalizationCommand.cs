using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Update;

public record UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(
    UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageParticipantsSectionLocalizationDto>>;
