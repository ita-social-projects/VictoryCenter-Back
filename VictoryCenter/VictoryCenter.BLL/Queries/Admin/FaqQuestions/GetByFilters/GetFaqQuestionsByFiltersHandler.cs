using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
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
        var translationStatusFilter = request.FaqQuestionsFilterDto.TranslationStatusFilter;
        var languageCount = await _repository.LocalizationLanguagesRepository.CountAsync();
        languageCount -= 1;
        Expression<Func<FaqQuestion, bool>> filter =
            (fq) => (status == null || fq.Status == status) && (pageId == null || fq.Placements.Any(p => p.PageId == pageId)) &&
            (translationStatusFilter == null ||
            translationStatusFilter == TranslationStatusFilter.All ||
            (translationStatusFilter == TranslationStatusFilter.Outdated &&
            fq.Localizations.Any(l => l.TranslationStatus == TranslationStatus.Outdated)) ||
            (translationStatusFilter == TranslationStatusFilter.Missing &&
            fq.Localizations.Count < languageCount));

        var queryOptions = new QueryOptions<FaqQuestion>
        {
            Include = fq => fq
            .Include(question => question.Placements)
            .Include(question => question.Localizations)
            .ThenInclude(loc => loc.Language),
            Offset = request.FaqQuestionsFilterDto.Offset is > 0 ?
            (int)request.FaqQuestionsFilterDto.Offset : 0,
            Limit = request.FaqQuestionsFilterDto.Limit is > 0 ?
            (int)request.FaqQuestionsFilterDto.Limit : 0,
            Filter = filter,
            OrderByASC = pageId != null ? t => t.Placements.Single(p => p.PageId == pageId).Priority : null,
        };

        var questions = await _repository.FaqQuestionsRepository.GetAllAsync(queryOptions);
        var questionsDto = _mapper.Map<FaqQuestionDto[]>(questions);
        var itemsTotalCount = await _repository.FaqQuestionsRepository.CountAsync(queryOptions with { Offset = 0, Limit = 0 });

        return Result.Ok(new PaginationResult<FaqQuestionDto>(questionsDto, itemsTotalCount));
    }
}
