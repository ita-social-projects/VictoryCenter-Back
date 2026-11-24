using AutoMapper;
using VictoryCenter.BLL.DTOs.Public.PdfSection;
using VictoryCenter.DAL.Entities.Localization;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.BLL.Mapping.PdfSection;

public class PublicPdfSectionProfile : Profile
{
    public PublicPdfSectionProfile()
    {
        CreateMap<PdfSectionEntity, PublicPdfSectionDto>()
            .ForMember(
                dest => dest.Localizations,
                opt => opt.MapFrom(src => (src.Localizations ?? Enumerable.Empty<PdfSectionLocalization>())
                    .Where(l => l.Language != null)));

        CreateMap<PdfSectionLocalization, PublicPdfSectionLocalizationDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
    }
}
