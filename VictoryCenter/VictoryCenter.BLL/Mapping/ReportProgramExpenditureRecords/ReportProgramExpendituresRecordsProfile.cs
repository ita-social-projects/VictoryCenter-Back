using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.ReportProgramExpenditureRecords;

public class ReportProgramExpendituresRecordsProfile : Profile
{
    public ReportProgramExpendituresRecordsProfile()
    {
        CreateMap<ReportProgramExpendituresRecord, ReportProgramExpendituresRecordDto>();
        CreateMap<ReportProgramExpendituresRecordDto, ReportProgramExpendituresRecord>();
        CreateMap<CreateReportProgramExpendituresRecordDto, ReportProgramExpendituresRecord>();
        CreateMap<UpdateReportProgramExpendituresRecordDto, ReportProgramExpendituresRecord>();
    }
}
