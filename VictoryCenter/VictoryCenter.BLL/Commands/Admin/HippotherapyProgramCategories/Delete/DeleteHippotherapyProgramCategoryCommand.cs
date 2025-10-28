using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Delete;

public record DeleteHippotherapyProgramCategoryCommand(long Id) : IRequest<Result<long>>;
