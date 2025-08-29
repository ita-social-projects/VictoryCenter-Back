using AutoMapper;
using VictoryCenter.BLL.DTOs.AboutUsSectionDto;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.AboutUsSections;

public class AboutUsSectionsProfile : Profile
{
    public AboutUsSectionsProfile()
    {
        CreateMap<AboutUsSection, AboutUsSectionDto>();
    }
}
