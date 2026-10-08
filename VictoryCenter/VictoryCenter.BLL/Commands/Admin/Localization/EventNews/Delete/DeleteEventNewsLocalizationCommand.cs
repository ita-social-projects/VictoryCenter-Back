using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Delete;

public record DeleteEventNewsLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteEventNewsLocalizationDto>>;
