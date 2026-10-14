using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.EventsIntroSection.GetByEntityId;

public record GetEventsIntroSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<EventsIntroSectionLocalizationDto>>>;
