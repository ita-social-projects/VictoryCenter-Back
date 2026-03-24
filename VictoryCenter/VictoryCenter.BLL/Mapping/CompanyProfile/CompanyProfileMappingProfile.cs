using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using CompanyProfileEntity = VictoryCenter.DAL.Entities.CompanyProfile;

namespace VictoryCenter.BLL.Mapping.CompanyProfile;

public class CompanyProfileMappingProfile : Profile
{
    public CompanyProfileMappingProfile()
    {
        CreateMap<CreateCompanyProfileDto, CompanyProfileEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Contacts))
            .ForMember(dest => dest.Requisite, opt => opt.MapFrom(src => src.Requisites))
            .ForMember(dest => dest.SocialLinks, opt => opt.MapFrom(src => src.SocialLinks));

        CreateMap<CreateCompanyProfileContactsDto, CompanyProfileContact>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
            .ForMember(dest => dest.Profile, opt => opt.Ignore())
            .ForMember(dest => dest.Localizations, opt => opt.Ignore());

        CreateMap<CreateCompanyProfileRequisiteDto, CompanyProfileRequisite>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
            .ForMember(dest => dest.Profile, opt => opt.Ignore())
            .ForMember(dest => dest.Localizations, opt => opt.Ignore());

        CreateMap<CreateSocialLinkDto, CompanyProfileSocialLink>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
            .ForMember(dest => dest.Profile, opt => opt.Ignore());

        CreateMap<CompanyProfileEntity, CompanyProfileDto>()
            .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contact))
            .ForMember(dest => dest.Requisites, opt => opt.MapFrom(src => src.Requisite));

        CreateMap<CompanyProfileContact, CompanyProfileContactsDto>();
        CreateMap<CompanyProfileRequisite, CompanyProfileRequisiteDto>();
        CreateMap<CompanyProfileSocialLink, SocialLinkDto>();

        CreateMap<CompanyProfileContactLocalization, CompanyProfileContactLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));

        CreateMap<CompanyProfileRequisiteLocalization, CompanyProfileRequisiteLocalizationDto>()
            .ForMember(dest => dest.LocalizationInfoDto, opt => opt.MapFrom(src => src.Language));

        CreateMap<CreateCompanyProfileContactLocalizationDto, CompanyProfileContactLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateCompanyProfileContactLocalizationDto, CompanyProfileContactLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<CreateCompanyProfileRequisiteLocalizationDto, CompanyProfileRequisiteLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateCompanyProfileRequisiteLocalizationDto, CompanyProfileRequisiteLocalization>()
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));
    }
}
