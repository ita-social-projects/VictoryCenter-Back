using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;

namespace VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Create;

public record CreateEventNewsCategoryCommand(CreateEventNewsCategoryDto Category)
    : IValidatableRequest<Result<AdminEventNewsCategoryDto>>;
