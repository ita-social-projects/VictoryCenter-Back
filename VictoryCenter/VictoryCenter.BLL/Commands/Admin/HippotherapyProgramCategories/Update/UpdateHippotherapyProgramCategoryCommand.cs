using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Update;

public record UpdateHippotherapyProgramCategoryCommand(UpdateHippotherapyProgramCategoryDto UpdateProgramCategoryDto, long Id)
    : IRequest<Result<HippotherapyProgramCategoryDto>>;
