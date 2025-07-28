using AutoMapper;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Programs;

public class ProgramsProfile : Profile
{
    public ProgramsProfile()
    {
        CreateMap<Program, ProgramDto>();
        CreateMap<Program, PublishedProgramDto>();
        CreateMap<CreateProgramDto, Program>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
        CreateMap<UpdateProgramDto, Program>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
    }
}
