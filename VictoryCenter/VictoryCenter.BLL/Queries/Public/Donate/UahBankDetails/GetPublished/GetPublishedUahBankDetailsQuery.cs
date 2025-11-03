using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Queries.Public.Donate.UahBankDetails.GetPublished;

public record GetPublishedUahBankDetailsQuery : IRequest<Result<List<PublishedUahBankDetailsDto>>>;
