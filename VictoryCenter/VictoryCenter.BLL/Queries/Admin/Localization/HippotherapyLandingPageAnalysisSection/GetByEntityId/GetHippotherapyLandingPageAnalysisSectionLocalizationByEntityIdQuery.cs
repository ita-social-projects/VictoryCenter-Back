using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageAnalysisSection.GetByEntityId;

public record GetHippotherapyLandingPageAnalysisSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageAnalysisSectionLocalizationDto>>>;
