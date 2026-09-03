using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetByFilters;

public class GetFeedbackReviewsByFiltersHandler
    : IRequestHandler<GetFeedbackReviewsByFiltersQuery, Result<PaginationResult<FeedbackReviewDto>>>
{
    private const int DefaultLimit = 20;

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
        var queryOptions = new QueryOptions<FeedbackReview>
        {
            Offset = request.Filter.Offset ?? 0,
            Limit = request.Filter.Limit ?? DefaultLimit,
            OrderByASC = review => review.Priority,
            AsNoTracking = true
        };

        var reviews = await _repositoryWrapper.FeedbackReviewsRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.FeedbackReviewsRepository.CountAsync(
            new QueryOptions<FeedbackReview> { AsNoTracking = true });

        var items = _mapper.Map<FeedbackReviewDto[]>(reviews);

        return Result.Ok(new PaginationResult<FeedbackReviewDto>(items, totalCount));
    }
}
