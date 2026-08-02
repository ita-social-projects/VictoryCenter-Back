using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.ReportFundsExpenditures;

namespace VictoryCenter.BLL.Queries.Public.ReportFundsExpenditures.GetPublished;

public record GetPublishedReportFundsExpendituresQuery(long? LanguageId)
    : IRequest<Result<PublishedReportFundsExpendituresDto>>;
