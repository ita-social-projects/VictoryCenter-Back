using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Delete;

public record DeleteHippotherapyLandingPageAnalysisSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteHippotherapyLandingPageAnalysisSectionLocalizationDto>>;
