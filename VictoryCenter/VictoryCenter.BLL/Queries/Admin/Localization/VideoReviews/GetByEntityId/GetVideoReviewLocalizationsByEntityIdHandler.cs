using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.VideoReviews.GetByEntityId;

public class GetVideoReviewLocalizationsByEntityIdHandler
    : IRequestHandler<
        GetVideoReviewLocalizationsByEntityIdQuery,
        Result<List<VideoReviewLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetVideoReviewLocalizationsByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<VideoReviewLocalizationDto>>> Handle(
        GetVideoReviewLocalizationsByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _repositoryWrapper.VideoReviewsRepository.ExistsAsync(
                videoReview => videoReview.Id == request.EntityId))
        {
            return Result.Fail<List<VideoReviewLocalizationDto>>(
                ErrorMessagesConstants.NotFound(request.EntityId, typeof(VideoReview)));
        }

        var localizations = await _repositoryWrapper.VideoReviewLocalizationsRepository.GetAllAsync(
            new QueryOptions<VideoReviewLocalization>
            {
                Filter = localization => localization.EntityId == request.EntityId,
                Include = query => query.Include(localization => localization.Language),
                OrderByASC = localization => localization.Language.Code,
                AsNoTracking = true
            });

        return Result.Ok(_mapper.Map<List<VideoReviewLocalizationDto>>(localizations));
    }
}
