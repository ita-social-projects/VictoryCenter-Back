using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;

namespace VictoryCenter.BLL.Commands.Admin.EventsPage.Update;

public record UpdateEventsPageDescriptionCommand(UpdateEventsPageDescriptionDto Dto) : IRequest<Result<EventsIntroSectionDto>>;
