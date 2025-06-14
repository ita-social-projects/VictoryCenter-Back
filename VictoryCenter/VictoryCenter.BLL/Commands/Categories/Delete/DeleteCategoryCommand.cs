using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Categories.DeleteCategory;

public record DeleteCategoryCommand(long id)
    : IRequest<Result<long>>;
