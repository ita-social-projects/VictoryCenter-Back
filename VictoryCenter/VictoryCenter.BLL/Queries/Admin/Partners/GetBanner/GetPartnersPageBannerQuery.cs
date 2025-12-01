using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Queries.Admin.Partners.GetBanner;

public record GetPartnersPageBannerQuery : IRequest<Result<PartnersPageBannerDto>>;
