using VictoryCenter.BLL.Interfaces.ReportExpendituresRecords;

namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

public record BatchSaveReportFundsExpendituresRecordsDto
    : IBatchSaveReportExpendituresRecordsDto<CreateReportFundsExpendituresRecordDto, BatchUpdateReportFundsExpendituresRecordDto>
{
    public List<CreateReportFundsExpendituresRecordDto> RecordsToCreate { get; init; } = [];

    public List<BatchUpdateReportFundsExpendituresRecordDto> RecordsToUpdate { get; init; } = [];

    public List<long> RecordIdsToDelete { get; init; } = [];
}
