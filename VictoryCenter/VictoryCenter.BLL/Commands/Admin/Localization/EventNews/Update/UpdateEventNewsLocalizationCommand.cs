using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;

public record UpdateEventNewsLocalizationCommand(
    UpdateEventNewsLocalizationDto UpdateEventNewsLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<EventNewsLocalizationDto>>;
