using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Queries.Donations.GetByCurrency;

public record GetByCurrencyQuery(Currency Currency) : IRequest<Result<DonationsByCurrencyDto>>;
