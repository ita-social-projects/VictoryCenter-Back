using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Update;

public class UpdateVideoReviewLocalizationHandler
    : IRequestHandler<UpdateVideoReviewLocalizationCommand, Result<VideoReviewLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateVideoReviewLocalizationHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<VideoReviewLocalizationDto>> Handle(
        UpdateVideoReviewLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.VideoReviewLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<VideoReviewLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId,
                Include = query => query.Include(entity => entity.Language),
                AsNoTracking = false
            });

        if (localization is null)
        {
            return Result.Fail<VideoReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(VideoReviewLocalization)));
        }

        var normalizedTitle = request.Localization.Title.Trim();

        if (string.Equals(localization.Title, normalizedTitle, StringComparison.Ordinal)
            && localization.TranslationStatus == TranslationStatus.Relevant)
        {
            return Result.Ok(_mapper.Map<VideoReviewLocalizationDto>(localization));
        }

        localization.Title = normalizedTitle;
        localization.TranslationStatus = TranslationStatus.Relevant;

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<VideoReviewLocalizationDto>(localization));
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<VideoReviewLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(VideoReviewLocalization)));
        }

        return Result.Fail<VideoReviewLocalizationDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(VideoReviewLocalization)));
    }
}
