using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.DTOs.Public.Donate.SupportOptions;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Donate;

public class SupportOptionsProfile : Profile
{
    public SupportOptionsProfile()
    {
        CreateMap<SupportOptions, SupportOptionsDto>();
        CreateMap<SupportOptions, PublishedSupportOptionsDto>();
        CreateMap<CreateSupportOptionsDto, SupportOptions>();
        CreateMap<UpdateSupportOptionsDto, SupportOptions>();
    }
}
