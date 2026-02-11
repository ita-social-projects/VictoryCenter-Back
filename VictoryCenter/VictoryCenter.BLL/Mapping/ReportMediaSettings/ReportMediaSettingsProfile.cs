using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.ReportMediaSettings;

public class ReportMediaSettingsProfile : Profile
{
    public ReportMediaSettingsProfile()
    {
        CreateMap<ChangedLivesBlock, ChangedLivesBlockDto>();
        CreateMap<CollectedFundsBlock, CollectedFundsBlockDto>();

        CreateMap<UpdateChangedLivesBlockDto, ChangedLivesBlock>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ChangedLivesCount, opt => opt.MapFrom(src => src.ChangedLives));

        CreateMap<UpdateCollectedFundsBlockDto, CollectedFundsBlock>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CollectedAmount, opt => opt.MapFrom(src => src.CollectedFunds));
    }
}
