using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.ProgramCategories;

namespace VictoryCenter.BLL.Commands.ProgramCategory.Create;

public record CreateProgramCategoryCommand(CreateProgramCategoryDto programCategoryDto)
    : IRequest<Result<ProgramCategoryDto>>;
