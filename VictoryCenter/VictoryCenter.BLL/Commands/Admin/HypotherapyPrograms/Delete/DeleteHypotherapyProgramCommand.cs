using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Delete;

public record DeleteHypotherapyProgramCommand(long Id) : IRequest<Result<long>>;
