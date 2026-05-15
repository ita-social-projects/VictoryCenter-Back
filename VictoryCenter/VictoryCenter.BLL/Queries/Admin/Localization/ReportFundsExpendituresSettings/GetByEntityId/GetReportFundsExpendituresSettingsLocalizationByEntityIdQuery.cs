using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Queries.Admin.Localization.ReportFundsExpendituresSettings.GetByEntityId;

public record GetReportFundsExpendituresSettingsLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<ReportFundsExpendituresSettingsLocalizationDto>>>;
