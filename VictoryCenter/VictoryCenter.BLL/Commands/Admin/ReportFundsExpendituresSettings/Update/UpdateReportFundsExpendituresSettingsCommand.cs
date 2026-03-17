using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresSettings.Update;

public record UpdateReportFundsExpendituresSettingsCommand(
    UpdateReportFundsExpendituresSettingsDto UpdateReportFundsExpendituresSettingsDto)
    : IRequest<Result<ReportFundsExpendituresSettingsDto>>;
