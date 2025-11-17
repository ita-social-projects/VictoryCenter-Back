using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.BLL.DTOs.Public.Donate.UahBankDetails;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Donate;

public class UahBankDetailsProfile : Profile
{
    public UahBankDetailsProfile()
    {
        CreateMap<UahBankDetails, UahBankDetailsDto>();
        CreateMap<UahBankDetails, PublishedUahBankDetailsDto>();
        CreateMap<CreateUahBankDetailsDto, UahBankDetails>();
        CreateMap<UpdateUahBankDetailsDto, UahBankDetails>();
    }
}
