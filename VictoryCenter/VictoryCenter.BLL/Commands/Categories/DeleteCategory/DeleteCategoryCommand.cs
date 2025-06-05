using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Categories.DeleteCategory;

public record DeleteCategoryCommand(int id)
    : IRequest<Result<int>>;
