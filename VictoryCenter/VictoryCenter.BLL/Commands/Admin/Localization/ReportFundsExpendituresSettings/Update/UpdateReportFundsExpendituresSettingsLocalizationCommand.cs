using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Update;

public record UpdateReportFundsExpendituresSettingsLocalizationCommand(
    UpdateReportFundsExpendituresSettingsLocalizationDto UpdateReportFundsExpendituresSettingsLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<ReportFundsExpendituresSettingsLocalizationDto>>;
