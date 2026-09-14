using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Create;

public class CreateFeedbackHistoryLocalizationHandler
    : IRequestHandler<CreateFeedbackHistoryLocalizationCommand, Result<FeedbackHistoryLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateFeedbackHistoryLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<FeedbackHistoryLocalizationDto>> Handle(
        CreateFeedbackHistoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Localization;

        if (!await _repositoryWrapper.FeedbackHistoriesRepository.ExistsAsync(
                feedbackHistory => feedbackHistory.Id == dto.EntityId))
        {
            return Result.Fail<FeedbackHistoryLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.EntityId, typeof(FeedbackHistory)));
        }

        var language = await _repositoryWrapper.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = entity => entity.Id == dto.LanguageId,
                AsNoTracking = true
            });

        if (language is null)
        {
            return Result.Fail<FeedbackHistoryLocalizationDto>(
                ErrorMessagesConstants.NotFound(dto.LanguageId, typeof(LocalizationLanguage)));
        }

        if (await _repositoryWrapper.FeedbackHistoryLocalizationsRepository.ExistsAsync(
                localization => localization.EntityId == dto.EntityId
                    && localization.LanguageId == dto.LanguageId))
        {
            return Result.Fail<FeedbackHistoryLocalizationDto>(
                FeedbackHistoryConstants.LocalizationAlreadyExists);
        }

        var newLocalization = new FeedbackHistoryLocalization
        {
            EntityId = dto.EntityId,
            LanguageId = dto.LanguageId,
            Title = dto.Title.Trim(),
            Story = dto.Story.Trim(),
            TranslationStatus = TranslationStatus.Relevant,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repositoryWrapper.FeedbackHistoryLocalizationsRepository.CreateAsync(newLocalization);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(new FeedbackHistoryLocalizationDto
                {
                    EntityId = newLocalization.EntityId,
                    Language = new LocalizationInfoDto
                    {
                        Id = language.Id,
                        Code = language.Code
                    },
                    Title = newLocalization.Title,
                    Story = newLocalization.Story,
                    TranslationStatus = newLocalization.TranslationStatus
                });
            }
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintException())
        {
            return Result.Fail<FeedbackHistoryLocalizationDto>(
                FeedbackHistoryConstants.LocalizationAlreadyExists);
        }

        return Result.Fail<FeedbackHistoryLocalizationDto>(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackHistoryLocalization)));
    }
}
