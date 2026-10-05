using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.FeedbackHistories.GetPublished;

public class GetPublishedFeedbackHistoriesHandler
    : IRequestHandler<GetPublishedFeedbackHistoriesQuery, Result<List<PublishedFeedbackHistoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedFeedbackHistoriesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedFeedbackHistoryDto>>> Handle(
        GetPublishedFeedbackHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var feedbackHistories = await _repositoryWrapper.FeedbackHistoriesRepository.GetAllAsync(
            new QueryOptions<FeedbackHistory>
            {
                Filter = history => history.Status == Status.Published,
                OrderByASC = history => history.Priority,
                Include = query => query
                    .Include(history => history.Image!)
                    .Include(history => history.Localizations)
                        .ThenInclude(localization => localization.Language),
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<PublishedFeedbackHistoryDto>>(feedbackHistories));
    }
}
