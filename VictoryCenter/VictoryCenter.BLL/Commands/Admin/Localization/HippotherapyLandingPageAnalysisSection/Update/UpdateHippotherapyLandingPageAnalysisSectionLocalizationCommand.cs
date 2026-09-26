using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Update;

public record UpdateHippotherapyLandingPageAnalysisSectionLocalizationCommand(
    UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyLandingPageAnalysisSectionLocalizationDto>>;
