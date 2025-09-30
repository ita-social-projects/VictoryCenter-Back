using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.DTOs.Public.HypotherapyPrograms;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.HypotherapyPrograms;

public class HypotherapyProgramsProfile : Profile
{
    public HypotherapyProgramsProfile()
    {
        CreateMap<HypotherapyProgram, HypotherapyProgramDto>();
        CreateMap<HypotherapyProgram, PublishedHypotherapyProgramDto>();
        CreateMap<CreateHypotherapyProgramDto, HypotherapyProgram>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
        CreateMap<HypotherapyUpdateProgramDto, HypotherapyProgram>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
    }
}
