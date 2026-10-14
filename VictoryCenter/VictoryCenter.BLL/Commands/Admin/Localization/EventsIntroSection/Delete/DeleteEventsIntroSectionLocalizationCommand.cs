using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Delete;

public record DeleteEventsIntroSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteEventsIntroSectionLocalizationDto>>;
