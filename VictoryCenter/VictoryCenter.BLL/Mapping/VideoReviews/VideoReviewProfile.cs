using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.VideoReviews;

public class VideoReviewProfile : Profile
{
    public VideoReviewProfile()
    {
        CreateMap<VideoReview, VideoReviewDto>();
        CreateMap<CreateVideoReviewDto, VideoReview>();
        CreateMap<UpdateVideoReviewDto, VideoReview>();
    }
}
