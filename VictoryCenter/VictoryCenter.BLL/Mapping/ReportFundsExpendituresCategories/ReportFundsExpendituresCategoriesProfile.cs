using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.ReportFundsExpendituresCategories;

public class ReportFundsExpendituresCategoriesProfile : Profile
{
    public ReportFundsExpendituresCategoriesProfile()
    {
        CreateMap<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryDto>();
        CreateMap<CreateReportFundsExpendituresCategoryDto, ReportFundsExpendituresCategory>();
        CreateMap<UpdateReportFundsExpendituresCategoryDto, ReportFundsExpendituresCategory>()
            .ForMember(dest => dest.Type, opt => opt.Ignore());
    }
}
