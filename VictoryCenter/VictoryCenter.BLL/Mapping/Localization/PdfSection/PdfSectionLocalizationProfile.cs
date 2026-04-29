using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.PdfSection;

public class PdfSectionLocalizationProfile : Profile
{
    public PdfSectionLocalizationProfile()
    {
        CreateMap<CreatePdfSectionLocalizationDto, PdfSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdatePdfSectionLocalizationDto, PdfSectionLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<PdfSectionLocalization, PdfSectionLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));
    }
}
