using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresSettings.Get;

public record GetReportFundsExpendituresSettingsQuery
    : IRequest<Result<ReportFundsExpendituresSettingsDto>>;
