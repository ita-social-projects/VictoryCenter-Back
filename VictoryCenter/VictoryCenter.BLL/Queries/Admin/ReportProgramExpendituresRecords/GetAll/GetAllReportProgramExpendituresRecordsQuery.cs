using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Queries.Admin.ReportProgramExpendituresRecords.GetAll;

public record GetAllReportProgramExpendituresRecordsQuery(IEnumerable<long>? HippotherapyProgramCategoryIds = null)
    : IRequest<Result<IEnumerable<ReportProgramExpendituresRecordDto>>>;
