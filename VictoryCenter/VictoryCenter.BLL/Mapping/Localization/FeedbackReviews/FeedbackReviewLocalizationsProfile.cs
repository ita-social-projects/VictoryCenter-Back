using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.Localization.FeedbackReviews;

public class FeedbackReviewLocalizationsProfile : Profile
{
    public FeedbackReviewLocalizationsProfile()
    {
        CreateMap<FeedbackReviewLocalization, FeedbackReviewLocalizationDto>()
            .ForMember(
                destination => destination.Language,
                options => options.MapFrom(source => source.Language));
    }
}
