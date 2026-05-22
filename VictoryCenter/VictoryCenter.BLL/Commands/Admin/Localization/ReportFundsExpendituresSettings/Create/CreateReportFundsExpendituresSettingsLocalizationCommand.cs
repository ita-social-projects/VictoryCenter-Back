using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Create;

public record CreateReportFundsExpendituresSettingsLocalizationCommand(
    CreateReportFundsExpendituresSettingsLocalizationDto CreateReportFundsExpendituresSettingsLocalizationDto)
    : IRequest<Result<ReportFundsExpendituresSettingsLocalizationDto>>;
