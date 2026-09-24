using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.VideoReviews.Search;

public class SearchVideoReviewHandler
    : IRequestHandler<SearchVideoReviewQuery, Result<PaginationResult<VideoReviewDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SearchVideoReviewQuery> _validator;
    private readonly ISearchService<VideoReview> _searchService;

    public SearchVideoReviewHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<SearchVideoReviewQuery> validator,
        ISearchService<VideoReview> searchService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _searchService = searchService;
    }

    public async Task<Result<PaginationResult<VideoReviewDto>>> Handle(
        SearchVideoReviewQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.SearchDto;
            var query = dto.SearchQuery.ToLower();

            var searchExpression = _searchService.CreateSearchExpression(
                CombineLogic.Or,
                new SearchTerm<VideoReview>
                {
                    TermSelector = review => (review.Title ?? string.Empty).ToLower(),
                    TermValue = query,
                    SearchLogic = SearchLogic.Contains,
                },
                new SearchTerm<VideoReview>
                {
                    TermSelector = review => (review.Link ?? string.Empty).ToLower(),
                    TermValue = query,
                    SearchLogic = SearchLogic.Contains,
                });

            var videoReviewsTask = _repositoryWrapper.VideoReviewsRepository.GetAllAsync(
                new QueryOptions<VideoReview>
                {
                    Filter = searchExpression,
                    OrderByASC = review => review.Priority,
                    Offset = dto.Offset is > 0 ? (int)dto.Offset : 0,
                    Limit = dto.Limit is > 0 ? (int)dto.Limit : 0,
                    Include = query => query
                        .Include(review => review.Localizations)
                            .ThenInclude(localization => localization.Language),
                });

            var countTask = _repositoryWrapper.VideoReviewsRepository.CountAsync(
                new QueryOptions<VideoReview> { Filter = searchExpression });

            await Task.WhenAll(videoReviewsTask, countTask);

            var videoReviews = await videoReviewsTask;
            var count = await countTask;

            var videoReviewDtos = _mapper.Map<List<VideoReviewDto>>(videoReviews);

            return Result.Ok(new PaginationResult<VideoReviewDto>(videoReviewDtos.ToArray(), count));
        }
        catch (ValidationException exception)
        {
            return Result.Fail(exception.Errors.Select(error => error.ErrorMessage));
        }
    }
}
