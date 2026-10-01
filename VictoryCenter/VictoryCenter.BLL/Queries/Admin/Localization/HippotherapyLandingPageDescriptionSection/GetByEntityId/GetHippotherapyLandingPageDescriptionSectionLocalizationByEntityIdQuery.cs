using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageDescriptionSection.GetByEntityId;

public record GetHippotherapyLandingPageDescriptionSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageDescriptionSectionLocalizationDto>>>;
