using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.FeedbackHistories;

public class FeedbackHistoriesProfile : Profile
{
    public FeedbackHistoriesProfile()
    {
        CreateMap<FeedbackHistory, FeedbackHistoryDto>();
        CreateMap<CreateFeedbackHistoryDto, FeedbackHistory>();
        CreateMap<UpdateFeedbackHistoryDto, FeedbackHistory>();
    }
}
