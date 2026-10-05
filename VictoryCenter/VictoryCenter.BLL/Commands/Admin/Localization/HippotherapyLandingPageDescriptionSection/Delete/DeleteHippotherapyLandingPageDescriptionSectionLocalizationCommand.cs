using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Delete;

public record DeleteHippotherapyLandingPageDescriptionSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyLandingPageDescriptionSectionLocalizationDto>>;
