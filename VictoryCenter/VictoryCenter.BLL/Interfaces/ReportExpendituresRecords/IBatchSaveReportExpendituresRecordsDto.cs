namespace VictoryCenter.BLL.Interfaces.ReportExpendituresRecords;

public interface IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>
{
    List<TCreateDto> RecordsToCreate { get; init; }

    List<TUpdateDto> RecordsToUpdate { get; init; }

    List<long> RecordIdsToDelete { get; init; }
}
