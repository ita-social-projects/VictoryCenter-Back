using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.ProgramCategories.Update;

public record UpdateProgramCategoryCommand(UpdateProgramCategoryDto updateProgramCategoryDto, long id)
    : IRequest<Result<ProgramCategoryDto>>;
