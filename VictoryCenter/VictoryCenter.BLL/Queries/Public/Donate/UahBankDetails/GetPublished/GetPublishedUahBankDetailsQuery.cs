using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Queries.Public.Donate.UahBankDetails.GetPublished;
public record GetPublishedUahBankDetailsQuery : IRequest<Result<List<UahBankDetailsDto>>>;
