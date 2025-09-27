using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Queries.Admin.Donate.ForeignBankDetails.GetAll;
public record GetAllForeignBankDetailsQuery : IRequest<Result<List<ForeignBankDetailsDto>>>;
