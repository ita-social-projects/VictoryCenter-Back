using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Create;

public record CreateHypotherapyProgramCategoryCommand(CreateHypotherapyProgramCategoryDto ProgramCategoryDto)
    : IRequest<Result<HypotherapyProgramCategoryDto>>;
