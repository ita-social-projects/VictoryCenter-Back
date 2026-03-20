using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.ReportFundsExpendituresRecords;

public class ReportFundsExpendituresRecordsProfile : Profile
{
    public ReportFundsExpendituresRecordsProfile()
    {
        CreateMap<ReportFundsExpendituresRecord, ReportFundsExpendituresRecordDto>();
        CreateMap<CreateReportFundsExpendituresRecordDto, ReportFundsExpendituresRecord>();
        CreateMap<UpdateReportFundsExpendituresRecordDto, ReportFundsExpendituresRecord>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.ReportingYear, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());
    }
}
