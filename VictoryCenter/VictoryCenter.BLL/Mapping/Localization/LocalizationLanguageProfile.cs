using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Localization;

public class LocalizationLanguageProfile : Profile
{
    public LocalizationLanguageProfile()
    {
        CreateMap<CreateLocalizationLanguageDto, LocalizationLanguage>();

        CreateMap<LocalizationLanguage, LocalizationLanguageDto>();

        CreateMap<UpdateLocalizationLanguageDto, LocalizationLanguageDto>();
    }
}
