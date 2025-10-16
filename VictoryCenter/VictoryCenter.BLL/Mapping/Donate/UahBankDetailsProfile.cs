using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Donate;
public class UahBankDetailsProfile : Profile
{
    public UahBankDetailsProfile()
    {
        CreateMap<UahBankDetails, UahBankDetailsDto>();
        CreateMap<CreateUahBankDetailsDto, UahBankDetails>();
        CreateMap<UpdateUahBankDetailsDto, UahBankDetails>();
    }
}
