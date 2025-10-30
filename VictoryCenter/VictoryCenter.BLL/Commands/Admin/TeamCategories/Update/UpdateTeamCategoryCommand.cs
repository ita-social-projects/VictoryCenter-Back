using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;

public record UpdateTeamCategoryCommand(UpdateTeamCategoryDto UpdateCategoryDto, long Id)
    : IRequest<Result<TeamCategoryDto>>;
