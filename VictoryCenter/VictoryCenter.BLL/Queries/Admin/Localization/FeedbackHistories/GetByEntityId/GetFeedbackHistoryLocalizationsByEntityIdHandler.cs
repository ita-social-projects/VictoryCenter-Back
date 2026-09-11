using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.FeedbackHistories.GetByEntityId;

public class GetFeedbackHistoryLocalizationsByEntityIdHandler
    : IRequestHandler<
        GetFeedbackHistoryLocalizationsByEntityIdQuery,
        Result<List<FeedbackHistoryLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetFeedbackHistoryLocalizationsByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<FeedbackHistoryLocalizationDto>>> Handle(
        GetFeedbackHistoryLocalizationsByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _repositoryWrapper.FeedbackHistoriesRepository.ExistsAsync(
                feedbackHistory => feedbackHistory.Id == request.EntityId))
        {
            return Result.Fail<List<FeedbackHistoryLocalizationDto>>(
                ErrorMessagesConstants.NotFound(request.EntityId, typeof(FeedbackHistory)));
        }

        var localizations = await _repositoryWrapper.FeedbackHistoryLocalizationsRepository.GetAllAsync(
            new QueryOptions<FeedbackHistoryLocalization>
            {
                Filter = localization => localization.EntityId == request.EntityId,
                Include = query => query.Include(localization => localization.Language),
                OrderByASC = localization => localization.Language.Code,
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<FeedbackHistoryLocalizationDto>>(localizations));
    }
}
