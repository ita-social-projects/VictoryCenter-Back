using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgramCategories.Create;

public record CreateHippotherapyProgramCategoryLocalizationCommand(
    CreateHippotherapyProgramCategoryLocalizationDto CreateHippotherapyProgramCategoryLocalizationDto)
    : IRequest<Result<HippotherapyProgramCategoryLocalizationDto>>;
