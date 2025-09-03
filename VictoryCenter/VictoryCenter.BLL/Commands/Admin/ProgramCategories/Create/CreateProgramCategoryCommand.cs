using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Create;

public record CreateProgramCategoryCommand(CreateProgramCategoryDto programCategoryDto)
    : IRequest<Result<ProgramCategoryDto>>;
