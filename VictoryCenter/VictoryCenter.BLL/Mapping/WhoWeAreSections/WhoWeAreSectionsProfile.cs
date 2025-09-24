using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.WhoWeAreSections;

public class WhoWeAreSectionsProfile : Profile
{
    public WhoWeAreSectionsProfile()
    {
        CreateMap<WhoWeAreSection, WhoWeAreSectionDto>();
        CreateMap<WhoWeAreSection, WhoWeArePageSectionDto>();
        CreateMap<WhoWeAreSection, WhoWeAreSectionInfoDto>()
            .ForMember(
                dest => dest.SectionType,
                opt => opt.MapFrom(src => src.SectionType.ToString()));
    }
}
