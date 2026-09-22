using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageIntroSection.GetByEntityId;

public record GetHippotherapyLandingPageIntroSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageIntroSectionLocalizationDto>>>;
