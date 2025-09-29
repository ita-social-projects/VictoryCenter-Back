using AutoMapper;
using VictoryCenter.BLL.Commands.Admin.Partners.Create;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Partners;

/*public class PartnerSectionsProfile : Profile
{
    public PartnerSectionsProfile()
    {
        CreateMap<UpdatePartnersSectionDto, PartnerSection>();

        CreateMap<PartnerSection, PartnersSectionDto>()
            .ForMember(
            dest => dest.Partners,
            opt => opt.MapFrom(src => src.Partners));

        CreateMap<CreatePartnerDto, Partner>();
    }
}*/

public class PartnerProfile : Profile
{
    public PartnerProfile()
    {
        CreateMap<CreatePartnersSectionDto, PartnerSection>();
        CreateMap<UpdatePartnersSectionDto, PartnerSection>();

        CreateMap<CreatePartnerDto, Partner>();
        CreateMap<UpdatePartnerDto, Partner>();

        CreateMap<Partner, PartnerDto>();
        CreateMap<PartnerSection, PartnersSectionDto>();
    }
}
