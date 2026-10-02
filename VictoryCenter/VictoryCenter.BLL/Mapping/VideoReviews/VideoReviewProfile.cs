using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.DTOs.Public.VideoReviews;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.VideoReviews;

public class VideoReviewProfile : Profile
{
    public VideoReviewProfile()
    {
        CreateMap<VideoReview, VideoReviewDto>();
        CreateMap<VideoReview, PublishedVideoReviewDto>();
        CreateMap<CreateVideoReviewDto, VideoReview>();
        CreateMap<UpdateVideoReviewDto, VideoReview>();
    }
}
