using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Create;

public record CreateEventsIntroSectionLocalizationCommand(
    CreateEventsIntroSectionLocalizationDto CreateEventsIntroSectionLocalizationDto)
    : IRequest<Result<EventsIntroSectionLocalizationDto>>;
