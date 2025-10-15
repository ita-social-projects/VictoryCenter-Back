using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Donate;
public class ForeignBankDetailsProfile : Profile
{
    public ForeignBankDetailsProfile()
    {
        CreateMap<ForeignBankDetails, ForeignBankDetailsDto>();
        CreateMap<CreateForeignBankDetailsDto, ForeignBankDetails>();
        CreateMap<UpdateForeignBankDetailsDto, ForeignBankDetails>()
            .ForMember(dest => dest.CorrespondentBanks, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
