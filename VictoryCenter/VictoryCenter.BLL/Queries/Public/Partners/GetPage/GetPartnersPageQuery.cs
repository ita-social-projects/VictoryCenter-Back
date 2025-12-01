using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Partners;

namespace VictoryCenter.BLL.Queries.Public.Partners.GetPage;

public record GetPartnersPageQuery : IRequest<Result<PartnersPageDto>>;
