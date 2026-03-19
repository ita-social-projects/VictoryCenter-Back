using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Mapping.ReportFundsExpendituresSettings;

public class ReportFundsExpendituresSettingsProfile : Profile
{
    public ReportFundsExpendituresSettingsProfile()
    {
        CreateMap<VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings, ReportFundsExpendituresSettingsDto>();
        CreateMap<CreateReportFundsExpendituresSettingsDto, VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings>();
        CreateMap<UpdateReportFundsExpendituresSettingsDto, VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
