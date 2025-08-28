using MediatR;
using FluentResults;
namespace VictoryCenter.BLL.Commands.Programs.Delete;

public record DeleteProgramCommand(long Id) : IRequest<Result<long>>;
