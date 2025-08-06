using MediatR;
using FluentResults;

namespace VictoryCenter.BLL.Commands.ProgramCategories.Delete;

public record DeleteProgramCategoryCommand(long id) : IRequest<Result<long>>;
