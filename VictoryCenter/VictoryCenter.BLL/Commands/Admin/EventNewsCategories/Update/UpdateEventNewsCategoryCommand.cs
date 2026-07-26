using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;

namespace VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Update;

public record UpdateEventNewsCategoryCommand(long Id, UpdateEventNewsCategoryDto Category)
    : IValidatableRequest<Result<AdminEventNewsCategoryDto>>;
