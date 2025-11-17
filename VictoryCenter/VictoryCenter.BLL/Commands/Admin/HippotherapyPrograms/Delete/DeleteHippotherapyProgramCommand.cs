using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Delete;

public record DeleteHippotherapyProgramCommand(long Id) : IRequest<Result<long>>;
