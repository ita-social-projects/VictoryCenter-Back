using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Create;

public class CreateVideoReviewLocalizationHandler
    : IRequestHandler<CreateVideoReviewLocalizationCommand, Result<VideoReviewLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateVideoReviewLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<VideoReviewLocalizationDto>> Handle(
        CreateVideoReviewLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Localization;

        if (!await _repositoryWrapper.VideoReviewsRepository.ExistsAsync(
                videoReview => videoReview.Id == dto.EntityId))
        {
            return Result.Fail<VideoReviewLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.EntityId, typeof(VideoReview)));
        }

        var language = await _repositoryWrapper.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = entity => entity.Id == dto.LanguageId,
                AsNoTracking = true
            });

        if (language is null)
        {
            return Result.Fail<VideoReviewLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.LanguageId, typeof(LocalizationLanguage)));
        }

        if (await _repositoryWrapper.VideoReviewLocalizationsRepository.ExistsAsync(
                localization => localization.EntityId == dto.EntityId
                    && localization.LanguageId == dto.LanguageId))
        {
            return Result.Fail<VideoReviewLocalizationDto>(
                VideoReviewConstants.LocalizationAlreadyExists);
        }

        var newLocalization = new VideoReviewLocalization
        {
            EntityId = dto.EntityId,
            LanguageId = dto.LanguageId,
            Title = dto.Title.Trim(),
            TranslationStatus = TranslationStatus.Relevant,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repositoryWrapper.VideoReviewLocalizationsRepository.CreateAsync(newLocalization);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(new VideoReviewLocalizationDto
                {
                    EntityId = newLocalization.EntityId,
                    Language = new LocalizationInfoDto
                    {
                        Id = language.Id,
                        Code = language.Code
                    },
                    Title = newLocalization.Title,
                    TranslationStatus = newLocalization.TranslationStatus
                });
            }
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintException())
        {
            return Result.Fail<VideoReviewLocalizationDto>(
                VideoReviewConstants.LocalizationAlreadyExists);
        }

        return Result.Fail<VideoReviewLocalizationDto>(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(VideoReviewLocalization)));
    }
}
