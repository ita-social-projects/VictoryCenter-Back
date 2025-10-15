using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Create;

public record CreateHippotherapyProgramCategoryCommand(CreateHippotherapyProgramCategoryDto ProgramCategoryDto)
    : IRequest<Result<HippotherapyProgramCategoryDto>>;
