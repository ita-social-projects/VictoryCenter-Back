using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Queries.Admin.ReportProgramExpendituresRecords.GetAll;

public record GetAllReportProgramExpendituresRecordsQuery(long? HippotherapyProgramCategoryId = null)
    : IRequest<Result<IEnumerable<ReportProgramExpendituresRecordDto>>>;
