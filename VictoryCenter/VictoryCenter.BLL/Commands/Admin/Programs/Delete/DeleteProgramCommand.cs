using FluentResults;
using MediatR;
namespace VictoryCenter.BLL.Commands.Admin.Programs.Delete;

public record DeleteProgramCommand(long Id) : IRequest<Result<long>>;
