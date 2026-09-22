using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetByFilters;

public class GetFeedbackReviewsByFiltersHandler
    : IRequestHandler<GetFeedbackReviewsByFiltersQuery, Result<PaginationResult<FeedbackReviewDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetFeedbackReviewsByFiltersHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PaginationResult<FeedbackReviewDto>>> Handle(
        GetFeedbackReviewsByFiltersQuery request,
        CancellationToken cancellationToken)
    {
        var translationStatusFilter = request.Filter.TranslationStatusFilter;
        var languageCount = await _repositoryWrapper.LocalizationLanguagesRepository.CountAsync();
        languageCount -= 1;

        Expression<Func<FeedbackReview, bool>> filter = review =>
            translationStatusFilter == null ||
            translationStatusFilter == TranslationStatusFilter.All ||
            (translationStatusFilter == TranslationStatusFilter.Outdated &&
                review.Localizations.Any(l => l.TranslationStatus == TranslationStatus.Outdated)) ||
            (translationStatusFilter == TranslationStatusFilter.Missing &&
                review.Localizations.Count < languageCount);

        var queryOptions = new QueryOptions<FeedbackReview>
        {
            Offset = request.Filter.Offset ?? 0,
            Limit = request.Filter.Limit ?? 0,
            OrderByASC = review => review.Priority,
            AsNoTracking = true,
            Include = q => q.Include(review => review.Localizations).ThenInclude(l => l.Language),
            Filter = filter
        };

        var reviews = await _repositoryWrapper.FeedbackReviewsRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.FeedbackReviewsRepository.CountAsync(
            queryOptions with { Offset = 0, Limit = 0 });

        var items = _mapper.Map<FeedbackReviewDto[]>(reviews);

        return Result.Ok(new PaginationResult<FeedbackReviewDto>(items, totalCount));
    }
}
