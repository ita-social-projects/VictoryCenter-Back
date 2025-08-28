using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Images.Delete;

public record DeleteImageCommand(long Id) : IRequest<Result<long>>;
