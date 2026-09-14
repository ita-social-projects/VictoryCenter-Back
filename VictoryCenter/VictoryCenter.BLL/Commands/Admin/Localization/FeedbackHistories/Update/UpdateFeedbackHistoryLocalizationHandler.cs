using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Update;

public class UpdateFeedbackHistoryLocalizationHandler
    : IRequestHandler<UpdateFeedbackHistoryLocalizationCommand, Result<FeedbackHistoryLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateFeedbackHistoryLocalizationHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<FeedbackHistoryLocalizationDto>> Handle(
        UpdateFeedbackHistoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.FeedbackHistoryLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<FeedbackHistoryLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId,
                Include = query => query.Include(entity => entity.Language),
                AsNoTracking = false
            });

        if (localization is null)
        {
            return Result.Fail<FeedbackHistoryLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackHistoryLocalization)));
        }

        var normalizedTitle = request.Localization.Title.Trim();
        var normalizedStory = request.Localization.Story.Trim();

        if (string.Equals(localization.Title, normalizedTitle, StringComparison.Ordinal)
            && string.Equals(localization.Story, normalizedStory, StringComparison.Ordinal)
            && localization.TranslationStatus == TranslationStatus.Relevant)
        {
            return Result.Ok(_mapper.Map<FeedbackHistoryLocalizationDto>(localization));
        }

        localization.Title = normalizedTitle;
        localization.Story = normalizedStory;
        localization.TranslationStatus = TranslationStatus.Relevant;

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<FeedbackHistoryLocalizationDto>(localization));
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<FeedbackHistoryLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(FeedbackHistoryLocalization)));
        }

        return Result.Fail<FeedbackHistoryLocalizationDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(FeedbackHistoryLocalization)));
    }
}
