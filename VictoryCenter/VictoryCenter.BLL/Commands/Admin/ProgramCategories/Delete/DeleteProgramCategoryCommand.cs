using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Delete;

public record DeleteProgramCategoryCommand(long Id) : IRequest<Result<long>>;
