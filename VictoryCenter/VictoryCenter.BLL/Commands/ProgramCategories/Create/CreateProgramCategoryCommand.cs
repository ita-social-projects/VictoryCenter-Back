using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.ProgramCategories;

namespace VictoryCenter.BLL.Commands.ProgramCategories.Create;

public record CreateProgramCategoryCommand(CreateProgramCategoryDto programCategoryDto)
    : IRequest<Result<ProgramCategoryDto>>;
