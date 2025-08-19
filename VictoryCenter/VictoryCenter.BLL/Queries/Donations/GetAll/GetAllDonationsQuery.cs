using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Donations.Common;

namespace VictoryCenter.BLL.Queries.Donations.GetAll;

public record GetAllDonationsQuery : IRequest<Result<DonationsGroupedDto>>;
