using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.ReportFundsExpendituresSettings;

public class ReportFundsExpendituresSettingsLocalizationProfile : Profile
{
    public ReportFundsExpendituresSettingsLocalizationProfile()
    {
        CreateMap<CreateReportFundsExpendituresSettingsLocalizationDto, ReportFundsExpendituresSettingsLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateReportFundsExpendituresSettingsLocalizationDto, ReportFundsExpendituresSettingsLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<ReportFundsExpendituresSettingsLocalization, ReportFundsExpendituresSettingsLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
