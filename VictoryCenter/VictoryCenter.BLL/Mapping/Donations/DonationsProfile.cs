using AutoMapper;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.BLL.DTOs.Donations.ForeignBank;
using VictoryCenter.BLL.DTOs.Donations.SupportMethod;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.Donations;

public class DonationsProfile : Profile
{
    public DonationsProfile()
    {
        CreateMap<AdditionalField, AdditionalFieldDto>();

        CreateMap<CorrespondentBank, CorrespondentBankDto>();

        CreateMap<SupportMethod, SupportMethodDto>();

        CreateMap<UahBankDetails, BankDetailsDto>()
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency.ToString()))
            .ForMember(dest => dest.SwiftCode, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.Ignore());

        CreateMap<ForeignBankDetails, BankDetailsDto>()
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency.ToString()))
            .ForMember(dest => dest.SwiftCode, opt => opt.MapFrom(src => src.SwiftCode))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address ?? string.Empty))
            .ForMember(dest => dest.Edrpou, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentPurpose, opt => opt.Ignore());
    }
}
