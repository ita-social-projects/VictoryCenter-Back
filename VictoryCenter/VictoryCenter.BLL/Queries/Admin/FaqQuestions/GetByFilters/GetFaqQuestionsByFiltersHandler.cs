using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FaqQuestions.GetByFilters;

public class GetFaqQuestionsByFiltersHandler : IRequestHandler<GetFaqQuestionsByFiltersQuery, Result<PaginationResult<FaqQuestionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetFaqQuestionsByFiltersHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<PaginationResult<FaqQuestionDto>>> Handle(GetFaqQuestionsByFiltersQuery request, CancellationToken cancellationToken)
    {
        var status = request.FaqQuestionsFilterDto.Status;
        var pageId = request.FaqQuestionsFilterDto.PageId;
        Expression<Func<FaqQuestion, bool>> filter =
            (fq) => (status == null || fq.Status == status) && (pageId == null || fq.Placements.Any(p => p.PageId == pageId));

        var queryOptions = new QueryOptions<FaqQuestion>
        {
            Include = fq => fq.Include(fq => fq.Placements),
            Offset = request.FaqQuestionsFilterDto.Offset is > 0 ?
            (int)request.FaqQuestionsFilterDto.Offset : 0,
            Limit = request.FaqQuestionsFilterDto.Limit is > 0 ?
            (int)request.FaqQuestionsFilterDto.Limit : 0,
            Filter = filter,
            OrderByASC = pageId != null ? t => t.Placements.Single(p => p.PageId == pageId).Priority : null,
        };

        var questions = await _repository.FaqQuestionsRepository.GetAllAsync(queryOptions);
        var questionsDto = _mapper.Map<FaqQuestionDto[]>(questions);
        var itemsTotalCount = await _repository.FaqQuestionsRepository.CountAsync(queryOptions.Filter);

        return Result.Ok(new PaginationResult<FaqQuestionDto>(questionsDto, itemsTotalCount));
    }
}
