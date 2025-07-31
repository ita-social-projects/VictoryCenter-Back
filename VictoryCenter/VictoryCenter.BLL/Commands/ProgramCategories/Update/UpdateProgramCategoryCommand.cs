using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.ProgramCategories;

namespace VictoryCenter.BLL.Commands.ProgramCategories.Update;

public record UpdateProgramCategoryCommand(UpdateProgramCategoryDto updateProgramCategoryDto)
    : IRequest<Result<ProgramCategoryDto>>;
