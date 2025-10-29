using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Delete;

public record DeleteTeamCategoryCommand(long Id)
    : IRequest<Result<long>>;
