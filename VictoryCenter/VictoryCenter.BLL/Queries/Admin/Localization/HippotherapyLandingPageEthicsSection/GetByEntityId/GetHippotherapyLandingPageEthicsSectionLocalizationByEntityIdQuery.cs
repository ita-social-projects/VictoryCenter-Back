using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageEthicsSection.GetByEntityId;

public record GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageEthicsSectionLocalizationDto>>>;
