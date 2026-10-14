using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Update;

public record UpdateEventsIntroSectionLocalizationCommand(
    UpdateEventsIntroSectionLocalizationDto UpdateEventsIntroSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<EventsIntroSectionLocalizationDto>>;
