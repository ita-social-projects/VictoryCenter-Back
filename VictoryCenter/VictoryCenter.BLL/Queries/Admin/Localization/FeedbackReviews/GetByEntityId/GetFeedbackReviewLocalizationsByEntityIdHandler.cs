using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.FeedbackReviews.GetByEntityId;

public class GetFeedbackReviewLocalizationsByEntityIdHandler
    : IRequestHandler<
        GetFeedbackReviewLocalizationsByEntityIdQuery,
        Result<List<FeedbackReviewLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetFeedbackReviewLocalizationsByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<FeedbackReviewLocalizationDto>>> Handle(
        GetFeedbackReviewLocalizationsByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _repositoryWrapper.FeedbackReviewsRepository.ExistsAsync(
                feedbackReview => feedbackReview.Id == request.EntityId))
        {
            return Result.Fail<List<FeedbackReviewLocalizationDto>>(
                ErrorMessagesConstants.NotFound(request.EntityId, typeof(FeedbackReview)));
        }

        var localizations = await _repositoryWrapper.FeedbackReviewLocalizationsRepository.GetAllAsync(
            new QueryOptions<FeedbackReviewLocalization>
            {
                Filter = localization => localization.EntityId == request.EntityId,
                Include = query => query.Include(localization => localization.Language),
                OrderByASC = localization => localization.Language.Code,
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<FeedbackReviewLocalizationDto>>(localizations));
    }
}
