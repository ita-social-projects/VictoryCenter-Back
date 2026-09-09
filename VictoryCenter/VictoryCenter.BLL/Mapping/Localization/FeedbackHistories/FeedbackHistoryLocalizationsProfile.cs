using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Mapping.Localization.FeedbackHistories;

public class FeedbackHistoryLocalizationsProfile : Profile
{
    public FeedbackHistoryLocalizationsProfile()
    {
        CreateMap<FeedbackHistoryLocalization, FeedbackHistoryLocalizationDto>()
            .ForMember(
                destination => destination.Language,
                options => options.MapFrom(source => source.Language));
    }
}
