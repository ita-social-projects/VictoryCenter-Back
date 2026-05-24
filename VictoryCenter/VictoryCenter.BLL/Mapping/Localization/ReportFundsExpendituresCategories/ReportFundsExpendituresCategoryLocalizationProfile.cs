using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.ReportFundsExpendituresCategories;

public class ReportFundsExpendituresCategoryLocalizationProfile : Profile
{
    public ReportFundsExpendituresCategoryLocalizationProfile()
    {
        CreateMap<CreateReportFundsExpendituresCategoryLocalizationDto, ReportFundsExpendituresCategoryLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateReportFundsExpendituresCategoryLocalizationDto, ReportFundsExpendituresCategoryLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<ReportFundsExpendituresCategoryLocalization, ReportFundsExpendituresCategoryLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
