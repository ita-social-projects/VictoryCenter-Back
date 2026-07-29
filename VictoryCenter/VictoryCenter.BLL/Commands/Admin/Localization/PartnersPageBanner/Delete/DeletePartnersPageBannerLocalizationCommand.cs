using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Delete;

public record DeletePartnersPageBannerLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeletePartnersPageBannerLocalizationDto>>;
