using AutoMapper;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.BLL.DTOs.Donations.ForeignBank;
using VictoryCenter.BLL.DTOs.Donations.SupportMethod;
using VictoryCenter.BLL.DTOs.Donations.UahBank;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Donations;

public class DonationsProfile : Profile
{
    public DonationsProfile()
    {
        CreateMap<AdditionalField, AdditionalFieldDto>();

        CreateMap<CorrespondentBank, CorrespondentBankDto>();

        CreateMap<SupportMethod, SupportMethodDto>();

        CreateMap<CreateAdditionalFieldDto, AdditionalField>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UahBankDetailsId, opt => opt.Ignore())
            .ForMember(dest => dest.ForeignBankDetailsId, opt => opt.Ignore())
            .ForMember(dest => dest.SupportMethodId, opt => opt.Ignore())
            .ForMember(dest => dest.UahBankDetails, opt => opt.Ignore())
            .ForMember(dest => dest.ForeignBankDetails, opt => opt.Ignore())
            .ForMember(dest => dest.SupportMethod, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<CreateCorrespondentBankDto, CorrespondentBank>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ForeignBankDetailsId, opt => opt.Ignore())
            .ForMember(dest => dest.ForeignBankDetails, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<CreateUahBankDetailsDto, UahBankDetails>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.AdditionalFields, opt => opt.MapFrom(src => src.AdditionalFields));

        CreateMap<CreateForeignBankDetailsDto, ForeignBankDetails>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => Enum.Parse<Currency>(src.Currency, true)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.AdditionalFields, opt => opt.MapFrom(src => src.AdditionalFields))
            .ForMember(dest => dest.CorrespondentBanks, opt => opt.MapFrom(src => src.CorrespondentBanks));

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
