using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.FeedbackHistories.GetLatestPublished;

public class GetLatestPublishedFeedbackHistoriesHandler
    : IRequestHandler<GetLatestPublishedFeedbackHistoriesQuery, Result<List<PublishedFeedbackHistoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetLatestPublishedFeedbackHistoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedFeedbackHistoryDto>>> Handle(
        GetLatestPublishedFeedbackHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var feedbackHistories = await _repositoryWrapper.FeedbackHistoriesRepository.GetAllAsync(
            new QueryOptions<FeedbackHistory>
            {
                Filter = history => history.Status == Status.Published && history.PublishedAt != null,
                OrderByDESC = history => history.PublishedAt!,
                ThenByDESC = history => history.CreatedAt,
                Limit = FeedbackHistoryConstants.LatestPublishedFeedbackHistoryCount,
                Include = query => query
                    .Include(history => history.Image!)
                    .Include(history => history.Localizations)
                        .ThenInclude(localization => localization.Language),
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<PublishedFeedbackHistoryDto>>(feedbackHistories));
    }
}
