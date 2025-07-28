using MediatR;
using FluentResults;

namespace VictoryCenter.BLL.Commands.ProgramCategory.Delete;

public record DeleteProgramCategoryCommand(long id) : IRequest<Result<long>>;
