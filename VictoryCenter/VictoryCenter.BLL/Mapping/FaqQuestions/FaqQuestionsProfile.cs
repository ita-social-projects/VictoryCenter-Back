using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Public.FAQ;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Mapping.FaqQuestions;

public class FaqQuestionsProfile : Profile
{
    public FaqQuestionsProfile()
    {
        CreateMap<CreateFaqQuestionDto, FaqQuestion>();

        CreateMap<UpdateFaqQuestionDto, FaqQuestion>();

        CreateMap<FaqQuestion, FaqQuestionDto>()
            .ForMember(dest => dest.PageIds, opt => opt.MapFrom(src => src.Placements.Select(p => p.PageId)));

        CreateMap<FaqQuestion, PublishedFaqQuestionDto>();
    }
}
