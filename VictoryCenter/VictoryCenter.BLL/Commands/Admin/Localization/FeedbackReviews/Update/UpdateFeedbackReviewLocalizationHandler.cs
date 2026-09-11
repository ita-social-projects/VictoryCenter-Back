using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Update;

public class UpdateFeedbackReviewLocalizationHandler
    : IRequestHandler<UpdateFeedbackReviewLocalizationCommand, Result<FeedbackReviewLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateFeedbackReviewLocalizationHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<FeedbackReviewLocalizationDto>> Handle(
        UpdateFeedbackReviewLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.FeedbackReviewLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FeedbackReviewLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId,
                Include = query => query.Include(entity => entity.Language),
                AsNoTracking = false
            });

        if (localization is null)
        {
            return Result.Fail<FeedbackReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackReviewLocalization)));
        }

        var normalizedAuthorName = request.Localization.AuthorName.Trim();
        var normalizedText = request.Localization.Text.Trim();

        if (string.Equals(localization.AuthorName, normalizedAuthorName, StringComparison.Ordinal)
            && string.Equals(localization.Text, normalizedText, StringComparison.Ordinal)
            && localization.TranslationStatus == TranslationStatus.Relevant)
        {
            return Result.Ok(_mapper.Map<FeedbackReviewLocalizationDto>(localization));
        }

        localization.AuthorName = normalizedAuthorName;
        localization.Text = normalizedText;
        localization.TranslationStatus = TranslationStatus.Relevant;

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<FeedbackReviewLocalizationDto>(localization));
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<FeedbackReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackReviewLocalization)));
        }

        return Result.Fail<FeedbackReviewLocalizationDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(FeedbackReviewLocalization)));
    }
}
