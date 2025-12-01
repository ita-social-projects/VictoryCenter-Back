using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Queries.Admin.Donate.ForeignBankDetails.GetAll;

public record GetAllForeignBankDetailsQuery(BankCurrency Currency) : IRequest<Result<List<ForeignBankDetailsDto>>>;
