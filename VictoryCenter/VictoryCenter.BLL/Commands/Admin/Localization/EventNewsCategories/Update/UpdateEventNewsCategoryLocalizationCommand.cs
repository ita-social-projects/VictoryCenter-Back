using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Update;

public record UpdateEventNewsCategoryLocalizationCommand(
    long EntityId,
    long LanguageId,
    UpdateEventNewsCategoryLocalizationDto Localization)
    : IValidatableRequest<Result<AdminEventNewsCategoryLocalizationDto>>;
