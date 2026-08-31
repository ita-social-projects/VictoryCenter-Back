using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.FeedbackReviews;

public class FeedbackReviewsProfile : Profile
{
    public FeedbackReviewsProfile()
    {
        CreateMap<FeedbackReview, FeedbackReviewDto>();

        CreateMap<CreateFeedbackReviewDto, FeedbackReview>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Priority, opt => opt.Ignore());
    }
}
