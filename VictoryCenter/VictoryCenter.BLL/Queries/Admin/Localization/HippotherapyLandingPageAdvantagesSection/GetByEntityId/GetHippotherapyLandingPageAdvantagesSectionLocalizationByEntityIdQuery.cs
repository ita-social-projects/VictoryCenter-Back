using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageAdvantagesSection.GetByEntityId;

public record GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>>;
