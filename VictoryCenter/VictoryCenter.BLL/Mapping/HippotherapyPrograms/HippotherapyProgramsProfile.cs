using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Public.HippotherapyPrograms;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.HippotherapyPrograms;

public class HippotherapyProgramsProfile : Profile
{
    public HippotherapyProgramsProfile()
    {
        CreateMap<HippotherapyProgram, HippotherapyProgramDto>();
        CreateMap<HippotherapyProgram, PublishedHippotherapyProgramDto>();
        CreateMap<CreateHippotherapyProgramDto, HippotherapyProgram>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.BackgroundImage, opt => opt.Ignore())
            .ForMember(dest => dest.PreviewImage, opt => opt.Ignore())
            .ForMember(dest => dest.Sections, opt => opt.Ignore());
        CreateMap<UpdateHippotherapyProgramDto, HippotherapyProgram>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.BackgroundImage, opt => opt.Ignore())
            .ForMember(dest => dest.PreviewImage, opt => opt.Ignore())
            .ForMember(dest => dest.Sections, opt => opt.Ignore());
    }
}
