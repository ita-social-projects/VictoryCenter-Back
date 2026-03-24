using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;

namespace VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;

public record CreateCompanyProfileCommand(CreateCompanyProfileDto CreateCompanyProfileDto) : IRequest<Result<CompanyProfileDto>>;
