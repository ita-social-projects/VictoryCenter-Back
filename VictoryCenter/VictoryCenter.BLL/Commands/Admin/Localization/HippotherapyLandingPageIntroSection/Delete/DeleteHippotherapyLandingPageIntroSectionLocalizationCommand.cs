using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Delete;

public record DeleteHippotherapyLandingPageIntroSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyLandingPageIntroSectionLocalizationDto>>;
