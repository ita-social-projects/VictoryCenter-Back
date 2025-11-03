using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;

namespace VictoryCenter.BLL.Commands.Admin.TeamCategories.Create;

public record CreateTeamCategoryCommand(CreateTeamCategoryDto CreateTeamCategoryDto)
    : IRequest<Result<TeamCategoryDto>>;
