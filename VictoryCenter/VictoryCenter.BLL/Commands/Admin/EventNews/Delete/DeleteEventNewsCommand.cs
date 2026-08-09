using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Delete;

public record DeleteEventNewsCommand(long Id) : IRequest<Result<long>>;
