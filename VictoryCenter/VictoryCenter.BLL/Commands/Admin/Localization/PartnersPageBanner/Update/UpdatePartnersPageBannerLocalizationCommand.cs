using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Update;

public record UpdatePartnersPageBannerLocalizationCommand(
    UpdatePartnersPageBannerLocalizationDto UpdatePartnersPageBannerLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<PartnersPageBannerLocalizationDto>>;
