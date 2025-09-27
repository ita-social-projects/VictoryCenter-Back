using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetById;

public class GetFaqQuestionByIdHandler : IRequestHandler<GetFaqQuestionByIdQuery, Result<FaqQuestionDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetFaqQuestionByIdHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<FaqQuestionDto>> Handle(GetFaqQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<FaqQuestion>
        {
            Filter = q => q.Id == request.Id,
            Include = q => q.Include(p => p.Placements)
        };

        var faqQuestion = await _repository.FaqQuestionsRepository.GetFirstOrDefaultAsync(queryOptions);

        if (faqQuestion == null)
        {
            return Result.Fail<FaqQuestionDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(FaqQuestion)));
        }

        return Result.Ok(_mapper.Map<FaqQuestionDto>(faqQuestion));
    }
}
