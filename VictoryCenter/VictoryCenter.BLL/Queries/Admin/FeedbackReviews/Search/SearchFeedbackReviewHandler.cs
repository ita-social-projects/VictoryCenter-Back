using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackReviews.Search;

public class SearchFeedbackReviewHandler
    : IRequestHandler<SearchFeedbackReviewQuery, Result<PaginationResult<FeedbackReviewDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SearchFeedbackReviewQuery> _validator;
    private readonly ISearchService<FeedbackReview> _searchService;

    public SearchFeedbackReviewHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<SearchFeedbackReviewQuery> validator,
        ISearchService<FeedbackReview> searchService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _searchService = searchService;
    }

    public async Task<Result<PaginationResult<FeedbackReviewDto>>> Handle(
        SearchFeedbackReviewQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.SearchDto;
            var query = dto.SearchQuery.ToLower();

            var authorNameExpression = _searchService.CreateSearchExpression(new SearchTerm<FeedbackReview>
            {
                TermSelector = review => (review.AuthorName ?? string.Empty).ToLower(),
                TermValue = query,
                SearchLogic = SearchLogic.Contains,
            });

            var textExpression = _searchService.CreateSearchExpression(new SearchTerm<FeedbackReview>
            {
                TermSelector = review => (review.Text ?? string.Empty).ToLower(),
                TermValue = query,
                SearchLogic = SearchLogic.Contains,
            });

            var searchExpression = authorNameExpression.Or(textExpression);

            var reviews = await _repositoryWrapper.FeedbackReviewsRepository.GetAllAsync(new QueryOptions<FeedbackReview>
            {
                Filter = searchExpression,
                OrderByASC = review => review.Priority,
                Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                Limit = dto.Limit is > 0 ? (int)dto.Limit : 0,
                Include = query => query.Include(review => review.Localizations).ThenInclude(localization => localization.Language),
            });
            var reviewDtos = _mapper.Map<List<FeedbackReviewDto>>(reviews);
            var count = await _repositoryWrapper.FeedbackReviewsRepository.CountAsync(
                new QueryOptions<FeedbackReview> { Filter = searchExpression });

            return Result.Ok(new PaginationResult<FeedbackReviewDto>(reviewDtos.ToArray(), count));
        }
        catch (ValidationException exception)
        {
            return Result.Fail(exception.Errors.Select(error => error.ErrorMessage));
        }
    }
}
