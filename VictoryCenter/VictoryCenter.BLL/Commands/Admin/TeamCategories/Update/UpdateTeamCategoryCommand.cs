using FluentResults;
using MediatR;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;

public record UpdateTeamCategoryCommand(UpdateTeamCategoryDto UpdateTeamCategoryDto, long Id)
    : IRequest<Result<TeamCategoryDto>>, IBaseValidatableRequest;
