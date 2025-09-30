using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Delete;

public record DeleteHypotherapyProgramCategoryCommand(long Id) : IRequest<Result<long>>;
