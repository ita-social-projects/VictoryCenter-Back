using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Delete;

public class DeleteVideoReviewLocalizationHandler
    : IRequestHandler<DeleteVideoReviewLocalizationCommand, Result<DeleteVideoReviewLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteVideoReviewLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteVideoReviewLocalizationDto>> Handle(
        DeleteVideoReviewLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.VideoReviewLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<VideoReviewLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId
            });

        if (localization is null)
        {
            return Result.Fail<DeleteVideoReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(VideoReviewLocalization)));
        }

        _repositoryWrapper.VideoReviewLocalizationsRepository.Delete(localization);

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(new DeleteVideoReviewLocalizationDto(request.EntityId, request.LanguageId))
                : Result.Fail<DeleteVideoReviewLocalizationDto>(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(VideoReviewLocalization)));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<DeleteVideoReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(VideoReviewLocalization)));
        }
    }
}
