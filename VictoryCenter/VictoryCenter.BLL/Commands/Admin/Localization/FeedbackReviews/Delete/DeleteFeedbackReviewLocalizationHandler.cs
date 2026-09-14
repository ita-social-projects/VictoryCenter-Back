using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Delete;

public class DeleteFeedbackReviewLocalizationHandler
    : IRequestHandler<DeleteFeedbackReviewLocalizationCommand, Result<DeleteFeedbackReviewLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteFeedbackReviewLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteFeedbackReviewLocalizationDto>> Handle(
        DeleteFeedbackReviewLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.FeedbackReviewLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FeedbackReviewLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId
            });

        if (localization is null)
        {
            return Result.Fail<DeleteFeedbackReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackReviewLocalization)));
        }

        _repositoryWrapper.FeedbackReviewLocalizationsRepository.Delete(localization);

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(new DeleteFeedbackReviewLocalizationDto(request.EntityId, request.LanguageId))
                : Result.Fail<DeleteFeedbackReviewLocalizationDto>(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackReviewLocalization)));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<DeleteFeedbackReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackReviewLocalization)));
        }
    }
}
