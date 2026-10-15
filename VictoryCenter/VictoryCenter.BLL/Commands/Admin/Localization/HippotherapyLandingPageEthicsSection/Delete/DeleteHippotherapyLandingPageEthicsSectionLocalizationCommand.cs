using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Delete;

public record DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>>;
