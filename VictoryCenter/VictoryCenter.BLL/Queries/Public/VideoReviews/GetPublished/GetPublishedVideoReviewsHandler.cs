using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.VideoReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.VideoReviews.GetPublished;

public class GetPublishedVideoReviewsHandler
    : IRequestHandler<GetPublishedVideoReviewsQuery, Result<List<PublishedVideoReviewDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedVideoReviewsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedVideoReviewDto>>> Handle(
        GetPublishedVideoReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var videoReviews = await _repositoryWrapper.VideoReviewsRepository.GetAllAsync(
            new QueryOptions<VideoReview>
            {
                Filter = videoReview => videoReview.Status == Status.Published && !videoReview.IsArchived,
                OrderByASC = videoReview => videoReview.Priority,
                Include = query => query
                    .Include(videoReview => videoReview.Localizations)
                        .ThenInclude(localization => localization.Language),
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<PublishedVideoReviewDto>>(videoReviews));
    }
}
