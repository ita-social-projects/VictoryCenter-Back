using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.EventNewsCategories;

public class EventNewsCategoryProfile : Profile
{
    public EventNewsCategoryProfile()
    {
        CreateMap<EventNewsCategory, AdminEventNewsCategoryDto>();
    }
}
