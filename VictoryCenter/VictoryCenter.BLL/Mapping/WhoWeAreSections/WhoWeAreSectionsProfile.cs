using AutoMapper;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.WhoWeAreSections;

public class WhoWeAreSectionsProfile : Profile
{
    public WhoWeAreSectionsProfile()
    {
        CreateMap<WhoWeAreSection, WhoWeAreSectionDto>();
        CreateMap<WhoWeAreSection, WhoWeAreSectionInfoDto>()
            .ForMember(
                dest => dest.SectionType,
                opt => opt.MapFrom(src => src.SectionType.ToString()));
        CreateMap<WhoWeAreSection, WhoWeArePageSectionDto>()
            .ForMember(
                dest => dest.SectionType,
                opt => opt.MapFrom(src => src.SectionType.ToString()));
    }
}
