using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;

namespace VictoryCenter.BLL.Mapping.HistorySections;

public class HistorySectionsProfile : Profile
{
    public HistorySectionsProfile()
    {
        CreateMap<HistorySection, HistorySectionDto>();

        CreateMap<HistorySectionContent, HistorySectionContentDto>()
            .Include<TitleHistoryContent, HistorySectionContentDto>()
            .Include<DescriptionHistoryContent, HistorySectionContentDto>()
            .Include<ImageHistoryContent, HistorySectionContentDto>();

        CreateMap<TitleHistoryContent, HistorySectionContentDto>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Description, opt => opt.Ignore())
            .ForMember(d => d.Image, opt => opt.Ignore());

        CreateMap<DescriptionHistoryContent, HistorySectionContentDto>()
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.Title, opt => opt.Ignore())
            .ForMember(d => d.Image, opt => opt.Ignore());

        CreateMap<ImageHistoryContent, HistorySectionContentDto>()
            .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Image))
            .ForMember(d => d.Title, opt => opt.Ignore())
            .ForMember(d => d.Description, opt => opt.Ignore());
    }
}
