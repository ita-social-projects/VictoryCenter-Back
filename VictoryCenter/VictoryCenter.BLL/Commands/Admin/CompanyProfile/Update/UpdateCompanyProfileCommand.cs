using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;

namespace VictoryCenter.BLL.Commands.Admin.CompanyProfile.Update;

public record UpdateCompanyProfileCommand(UpdateCompanyProfileDto UpdateCompanyProfileDto) : IRequest<Result<CompanyProfileDto>>;
