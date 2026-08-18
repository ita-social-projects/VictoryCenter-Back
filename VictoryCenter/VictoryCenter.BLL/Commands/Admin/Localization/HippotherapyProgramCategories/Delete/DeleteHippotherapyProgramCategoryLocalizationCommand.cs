using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgramCategories.Delete;

public record DeleteHippotherapyProgramCategoryLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyProgramCategoryLocalizationDto>>;
