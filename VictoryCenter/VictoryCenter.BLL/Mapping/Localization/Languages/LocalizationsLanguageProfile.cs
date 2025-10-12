using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.Localization.Languages;

public class LocalizationsLanguageProfile : Profile
{
    public LocalizationsLanguageProfile()
    {
        CreateMap<CreateLocalizationLanguageDto, LocalizationLanguage>();

        CreateMap<LocalizationLanguage, LocalizationLanguageDto>();

        CreateMap<UpdateLocalizationLanguageDto, LocalizationLanguage>();
    }
}
