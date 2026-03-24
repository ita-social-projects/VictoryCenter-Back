using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;

namespace VictoryCenter.BLL.Queries.Public.CompanyProfile.Get;

public record GetCompanyProfileQuery : IRequest<Result<CompanyProfileDto>>;
