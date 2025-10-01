using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.VisitorPages;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.VisitorPages;

public class VisitorPagesProfile : Profile
{
    public VisitorPagesProfile()
    {
        CreateMap<VisitorPage, VisitorPageDto>();
    }
}
