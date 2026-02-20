using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

namespace VictoryCenter.BLL.Commands.Admin.ReportMediaSettings.UpdateReportMediaSettings;

public record UpdateReportMediaSettingsCommand(UpdateReportMediaSettingsDto Dto)
    : IRequest<Result<ReportMediaSettingsDto>>;
