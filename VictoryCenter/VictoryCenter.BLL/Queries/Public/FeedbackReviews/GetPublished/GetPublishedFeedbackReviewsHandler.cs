using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.FeedbackReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.FeedbackReviews.GetPublished;

public class GetPublishedFeedbackReviewsHandler
    : IRequestHandler<GetPublishedFeedbackReviewsQuery, Result<List<PublishedFeedbackReviewDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedFeedbackReviewsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedFeedbackReviewDto>>> Handle(
        GetPublishedFeedbackReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var feedbackReviews = await _repositoryWrapper.FeedbackReviewsRepository.GetAllAsync(
            new QueryOptions<FeedbackReview>
            {
                Filter = review => review.Status == Status.Published,
                OrderByASC = review => review.Priority,
                Include = query => query
                    .Include(review => review.Localizations)
                        .ThenInclude(localization => localization.Language),
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<PublishedFeedbackReviewDto>>(feedbackReviews));
    }
}
