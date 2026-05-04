using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.History;

public class HistoryLocalizationProfile : Profile
{
    public HistoryLocalizationProfile()
    {
        CreateMap<CreateHistorySectionContentLocalizationDto, HistorySectionContentLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<HistorySectionContentLocalization, HistorySectionContentLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
