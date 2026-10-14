using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Delete;

public record DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>>;
