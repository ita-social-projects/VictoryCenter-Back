using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;

namespace VictoryCenter.BLL.Mapping.HippotherapyProgramSections;

public class HippotherapyProgramSectionsProfile : Profile
{
    public HippotherapyProgramSectionsProfile()
    {
        CreateMap<HippotherapyProgramSection, HippotherapyProgramSectionDto>();

        CreateMap<ProgramSectionContent, HippotherapyProgramSectionContentDto>()
            .Include<TitleProgramContent, HippotherapyProgramSectionContentDto>()
            .Include<DescriptionProgramContent, HippotherapyProgramSectionContentDto>()
            .Include<ImageProgramContent, HippotherapyProgramSectionContentDto>()
            .Include<AuthorProgramContent, HippotherapyProgramSectionContentDto>();

        CreateMap<TitleProgramContent, HippotherapyProgramSectionContentDto>()
            .ForMember(d => d.Description, opt => opt.Ignore())
            .ForMember(d => d.Image, opt => opt.Ignore())
            .ForMember(d => d.Author, opt => opt.Ignore());

        CreateMap<DescriptionProgramContent, HippotherapyProgramSectionContentDto>()
            .ForMember(d => d.Title, opt => opt.Ignore())
            .ForMember(d => d.Image, opt => opt.Ignore())
            .ForMember(d => d.Author, opt => opt.Ignore());

        CreateMap<ImageProgramContent, HippotherapyProgramSectionContentDto>()
            .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Image))
            .ForMember(d => d.Title, opt => opt.Ignore())
            .ForMember(d => d.Description, opt => opt.Ignore())
            .ForMember(d => d.Author, opt => opt.Ignore());

        CreateMap<AuthorProgramContent, HippotherapyProgramSectionContentDto>()
            .ForMember(d => d.Author, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Title, opt => opt.Ignore())
            .ForMember(d => d.Description, opt => opt.Ignore())
            .ForMember(d => d.Image, opt => opt.Ignore());
    }
}
