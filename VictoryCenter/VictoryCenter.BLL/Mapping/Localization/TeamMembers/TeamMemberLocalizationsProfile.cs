using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Mapping.Localization.TeamMembers;

public class TeamMemberLocalizationsProfile : Profile
{
    public TeamMemberLocalizationsProfile()
    {
        CreateMap<CreateTeamMemberLocalizationDto, TeamMemberLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.TeamMemberId))
            .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(_ => TranslationStatus.Relevant));

        CreateMap<UpdateTeamMemberLocalizationDto, TeamMemberLocalization>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.TeamMemberId))
            .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore());

        CreateMap<TeamMemberLocalization, TeamMemberLocalizationDto>()
            .ForMember(dest => dest.TeamMemberId, opt => opt.MapFrom(src => src.EntityId))
            .ForMember(dest => dest.LocalizationLanguageDto, opt => opt.MapFrom(src => src.Language));
    }
}
