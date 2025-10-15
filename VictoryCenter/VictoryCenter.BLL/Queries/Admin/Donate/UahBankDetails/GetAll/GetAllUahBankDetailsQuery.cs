using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Queries.Admin.Donate.UahBankDetails.GetAll;
public record GetAllUahBankDetailsQuery : IRequest<Result<List<UahBankDetailsDto>>>;
