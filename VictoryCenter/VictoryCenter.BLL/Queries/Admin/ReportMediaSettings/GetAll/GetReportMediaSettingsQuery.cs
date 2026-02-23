using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;

namespace VictoryCenter.BLL.Queries.Admin.ReportMediaSettings.GetAll;

public record GetReportMediaSettingsQuery : IRequest<Result<ReportMediaSettingsDto>>;
