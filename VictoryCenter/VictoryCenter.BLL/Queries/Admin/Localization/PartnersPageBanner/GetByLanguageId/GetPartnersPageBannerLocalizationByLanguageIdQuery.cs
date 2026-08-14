using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PartnersPageBanner.GetByLanguageId;

public record GetPartnersPageBannerLocalizationByLanguageIdQuery(long EntityId, long LanguageId)
    : IRequest<Result<PartnersPageBannerLocalizationDto>>;
