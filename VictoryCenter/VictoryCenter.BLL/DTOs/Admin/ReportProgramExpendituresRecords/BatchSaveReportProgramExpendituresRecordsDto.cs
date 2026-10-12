using VictoryCenter.BLL.Interfaces.ReportExpendituresRecords;

namespace VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

public record BatchSaveReportProgramExpendituresRecordsDto
    : IBatchSaveReportExpendituresRecordsDto<CreateReportProgramExpendituresRecordDto, BatchUpdateReportProgramExpendituresRecordDto>
{
    public List<CreateReportProgramExpendituresRecordDto> RecordsToCreate { get; init; } = [];

    public List<BatchUpdateReportProgramExpendituresRecordDto> RecordsToUpdate { get; init; } = [];

    public List<long> RecordIdsToDelete { get; init; } = [];
}
