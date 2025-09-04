using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Categories.Delete;

public record DeleteCategoryCommand(long Id)
    : IRequest<Result<long>>;
