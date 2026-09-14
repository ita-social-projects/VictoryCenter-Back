using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Delete;

public class DeleteFeedbackHistoryLocalizationHandler
    : IRequestHandler<DeleteFeedbackHistoryLocalizationCommand, Result<DeleteFeedbackHistoryLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteFeedbackHistoryLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteFeedbackHistoryLocalizationDto>> Handle(
        DeleteFeedbackHistoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.FeedbackHistoryLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FeedbackHistoryLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId
            });

        if (localization is null)
        {
            return Result.Fail<DeleteFeedbackHistoryLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackHistoryLocalization)));
        }

        _repositoryWrapper.FeedbackHistoryLocalizationsRepository.Delete(localization);

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(new DeleteFeedbackHistoryLocalizationDto(request.EntityId, request.LanguageId))
                : Result.Fail<DeleteFeedbackHistoryLocalizationDto>(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackHistoryLocalization)));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<DeleteFeedbackHistoryLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackHistoryLocalization)));
        }
    }
}
