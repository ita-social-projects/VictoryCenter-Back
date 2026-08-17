using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Update;

public record UpdateEventNewsCommand(long Id, UpdateEventNewsDto EventNews)
    : IValidatableRequest<Result<EventNewsDto>>;
