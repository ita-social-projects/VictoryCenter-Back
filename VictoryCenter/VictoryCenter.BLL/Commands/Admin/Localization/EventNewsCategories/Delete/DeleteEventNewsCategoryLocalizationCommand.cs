using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Delete;

public record DeleteEventNewsCategoryLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteEventNewsCategoryLocalizationDto>>;
