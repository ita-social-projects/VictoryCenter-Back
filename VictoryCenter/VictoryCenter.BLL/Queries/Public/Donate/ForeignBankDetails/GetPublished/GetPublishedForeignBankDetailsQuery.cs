using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Queries.Public.Donate.ForeignBankDetails.GetPublished;
public record GetPublishedForeignBankDetailsQuery(BankCurrency Currency) : IRequest<Result<List<ForeignBankDetailsDto>>>;
