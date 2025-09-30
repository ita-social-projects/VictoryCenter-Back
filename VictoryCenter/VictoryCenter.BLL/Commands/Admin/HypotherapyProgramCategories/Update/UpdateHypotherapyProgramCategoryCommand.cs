using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Update;

public record UpdateHypotherapyProgramCategoryCommand(UpdateHypotherapyProgramCategoryDto UpdateProgramCategoryDto, long Id)
    : IRequest<Result<HypotherapyProgramCategoryDto>>;
