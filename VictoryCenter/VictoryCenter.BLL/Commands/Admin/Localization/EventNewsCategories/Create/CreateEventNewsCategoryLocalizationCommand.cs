using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Create;

public record CreateEventNewsCategoryLocalizationCommand(CreateEventNewsCategoryLocalizationDto Localization)
    : IValidatableRequest<Result<AdminEventNewsCategoryLocalizationDto>>;
