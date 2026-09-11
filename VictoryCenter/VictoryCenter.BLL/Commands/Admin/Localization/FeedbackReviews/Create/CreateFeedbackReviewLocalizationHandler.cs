using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Create;

public class CreateFeedbackReviewLocalizationHandler
    : IRequestHandler<CreateFeedbackReviewLocalizationCommand, Result<FeedbackReviewLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateFeedbackReviewLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<FeedbackReviewLocalizationDto>> Handle(
        CreateFeedbackReviewLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Localization;

        if (!await _repositoryWrapper.FeedbackReviewsRepository.ExistsAsync(
                feedbackReview => feedbackReview.Id == dto.EntityId))
        {
            return Result.Fail<FeedbackReviewLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.EntityId, typeof(FeedbackReview)));
        }

        var language = await _repositoryWrapper.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = entity => entity.Id == dto.LanguageId,
                AsNoTracking = true
            });

        if (language is null)
        {
            return Result.Fail<FeedbackReviewLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.LanguageId, typeof(LocalizationLanguage)));
        }

        if (await _repositoryWrapper.FeedbackReviewLocalizationsRepository.ExistsAsync(
                localization => localization.EntityId == dto.EntityId
                    && localization.LanguageId == dto.LanguageId))
        {
            return Result.Fail<FeedbackReviewLocalizationDto>(
                FeedbackReviewConstants.LocalizationAlreadyExists);
        }

        var newLocalization = new FeedbackReviewLocalization
        {
            EntityId = dto.EntityId,
            LanguageId = dto.LanguageId,
            AuthorName = dto.AuthorName.Trim(),
            Text = dto.Text.Trim(),
            TranslationStatus = TranslationStatus.Relevant,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repositoryWrapper.FeedbackReviewLocalizationsRepository.CreateAsync(newLocalization);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(new FeedbackReviewLocalizationDto
                {
                    EntityId = newLocalization.EntityId,
                    Language = new LocalizationInfoDto
                    {
                        Id = language.Id,
                        Code = language.Code
                    },
                    AuthorName = newLocalization.AuthorName,
                    Text = newLocalization.Text,
                    TranslationStatus = newLocalization.TranslationStatus
                });
            }
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintException())
        {
            return Result.Fail<FeedbackReviewLocalizationDto>(
                FeedbackReviewConstants.LocalizationAlreadyExists);
        }

        return Result.Fail<FeedbackReviewLocalizationDto>(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackReviewLocalization)));
    }
}
