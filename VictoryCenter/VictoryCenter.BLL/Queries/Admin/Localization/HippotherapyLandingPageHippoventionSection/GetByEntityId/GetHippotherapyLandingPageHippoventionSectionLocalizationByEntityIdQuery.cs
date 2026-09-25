using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageHippoventionSection.GetByEntityId;

public record GetHippotherapyLandingPageHippoventionSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageHippoventionSectionLocalizationDto>>>;
