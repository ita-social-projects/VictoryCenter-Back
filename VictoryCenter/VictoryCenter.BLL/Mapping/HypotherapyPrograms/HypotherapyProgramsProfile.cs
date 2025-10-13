using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.DTOs.Public.HypotherapyPrograms;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.HypotherapyPrograms;

public class HypotherapyProgramsProfile : Profile
{
    public HypotherapyProgramsProfile()
    {
        CreateMap<HippotherapyProgram, HypotherapyProgramDto>();
        CreateMap<HippotherapyProgram, PublishedHypotherapyProgramDto>();
        CreateMap<CreateHypotherapyProgramDto, HippotherapyProgram>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
        CreateMap<HypotherapyUpdateProgramDto, HippotherapyProgram>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
    }
}
