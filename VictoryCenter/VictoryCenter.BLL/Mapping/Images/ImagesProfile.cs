using AutoMapper;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Images;

public class ImagesProfile : Profile
{
    public ImagesProfile()
    {
        CreateMap<CreateImageDTO, Image>();
        CreateMap<Image, ImageDTO>();
        CreateMap<UpdateImageDTO, Image>();
    }
}
