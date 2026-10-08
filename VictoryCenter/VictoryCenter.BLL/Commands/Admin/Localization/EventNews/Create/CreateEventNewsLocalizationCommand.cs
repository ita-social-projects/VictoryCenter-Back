using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;

public record CreateEventNewsLocalizationCommand(CreateEventNewsLocalizationDto CreateEventNewsLocalizationDto)
    : IRequest<Result<EventNewsLocalizationDto>>;
