using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.FeedbackHistories;

public class FeedbackHistoriesProfile : Profile
{
    public FeedbackHistoriesProfile()
    {
        CreateMap<FeedbackHistory, FeedbackHistoryDto>();
        CreateMap<FeedbackHistory, PublishedFeedbackHistoryDto>();
        CreateMap<CreateFeedbackHistoryDto, FeedbackHistory>();
        CreateMap<UpdateFeedbackHistoryDto, FeedbackHistory>();
    }
}
