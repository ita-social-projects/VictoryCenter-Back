using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Create;

public record CreateEventNewsCommand(CreateEventNewsDto CreateEventNewsDto)
    : IValidatableRequest<Result<EventNewsDto>>;
