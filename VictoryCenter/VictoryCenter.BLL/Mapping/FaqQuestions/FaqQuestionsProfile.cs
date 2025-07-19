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

        CreateMap<FaqQuestion, FaqQuestionDto>();

        CreateMap<FaqQuestion, PublishedFaqQuestionDto>();
    }
}
