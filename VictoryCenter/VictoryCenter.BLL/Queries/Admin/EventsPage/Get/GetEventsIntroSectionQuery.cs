using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;

namespace VictoryCenter.BLL.Queries.Admin.EventsPage.Get;

public record GetEventsIntroSectionQuery : IRequest<Result<EventsIntroSectionDto>>;
