using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
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
            var searchTerm = new SearchTerm<FeedbackReview>
            {
                TermSelector = review => (review.AuthorName ?? string.Empty).ToLower(),
                TermValue = dto.SearchQuery.ToLower(),
                SearchLogic = SearchLogic.Contains,
            };
            var searchExpression = _searchService.CreateSearchExpression(searchTerm);

            var reviews = await _repositoryWrapper.FeedbackReviewsRepository.GetAllAsync(new QueryOptions<FeedbackReview>
            {
                Filter = searchExpression,
                Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                Limit = dto.Limit is > 0 ? (int)dto.Limit : 0,
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
