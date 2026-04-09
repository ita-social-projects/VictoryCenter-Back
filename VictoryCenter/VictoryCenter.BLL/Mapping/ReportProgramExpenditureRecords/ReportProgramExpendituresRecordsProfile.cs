using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.ReportProgramExpenditureRecords;

public class ReportProgramExpendituresRecordsProfile : Profile
{
    protected ReportProgramExpendituresRecordsProfile()
    {
        CreateMap<ReportProgramExpendituresRecord, ReportProgramExpendituresRecordDto>();
        CreateMap<ReportProgramExpendituresRecordDto, ReportProgramExpendituresRecord>();
    }
}
