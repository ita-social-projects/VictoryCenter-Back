using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.Localization.VideoReviews;

public class VideoReviewLocalizationsProfile : Profile
{
    public VideoReviewLocalizationsProfile()
    {
        CreateMap<VideoReviewLocalization, VideoReviewLocalizationDto>()
            .ForMember(
                destination => destination.Language,
                options => options.MapFrom(source => source.Language));
    }
}
