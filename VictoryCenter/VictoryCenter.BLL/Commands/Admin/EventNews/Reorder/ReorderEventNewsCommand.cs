using FluentResults;
using MediatR;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Reorder;

public record ReorderEventNewsCommand(ReorderEventNewsDto Dto)
    : IValidatableRequest<Result<Unit>>;
