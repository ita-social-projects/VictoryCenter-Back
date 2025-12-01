using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;

public record UpdatePartnersPageBannerCommand(UpdatePartnersPageBannerDto Dto)
    : IRequest<Result<PartnersPageBannerDto>>;
