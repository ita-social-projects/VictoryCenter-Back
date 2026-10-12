using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;

public record CreateEventNewsLocalizationCommand(CreateAdminEventNewsLocalizationDto CreateAdminEventNewsLocalizationDto)
    : IRequest<Result<AdminEventNewsLocalizationDto>>;
