using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Create;

public record CreatePartnersPageBannerLocalizationCommand(CreatePartnersPageBannerLocalizationDto CreatePartnersPageBannerLocalizationDto)
    : IRequest<Result<PartnersPageBannerLocalizationDto>>;
