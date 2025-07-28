using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.ProgramCategories;

namespace VictoryCenter.BLL.Commands.ProgramCategory.Update;

public record UpdateProgramCategoryCommand(UpdateProgramCategoryDto updateProgramCategoryDto)
    : IRequest<Result<ProgramCategoryDto>>;
