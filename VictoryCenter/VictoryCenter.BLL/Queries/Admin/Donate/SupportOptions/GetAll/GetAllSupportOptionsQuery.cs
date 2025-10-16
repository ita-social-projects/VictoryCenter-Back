using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Queries.Admin.Donate.SupportOptions.GetAll;
public record GetAllSupportOptionsQuery(BankCurrency Currency) : IRequest<Result<List<SupportOptionsDto>>>;
