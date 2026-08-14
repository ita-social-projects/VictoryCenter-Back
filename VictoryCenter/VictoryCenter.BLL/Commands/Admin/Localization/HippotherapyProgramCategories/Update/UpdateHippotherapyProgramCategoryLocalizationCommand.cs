using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgramCategories.Update;

public record UpdateHippotherapyProgramCategoryLocalizationCommand(
    UpdateHippotherapyProgramCategoryLocalizationDto UpdateHippotherapyProgramCategoryLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyProgramCategoryLocalizationDto>>;
