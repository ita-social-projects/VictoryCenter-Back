using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.VideoReviews.GetAll;

public class GetAllVideoReviewsHandler : IRequestHandler<GetAllVideoReviewsQuery, Result<List<VideoReviewDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllVideoReviewsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<VideoReviewDto>>> Handle(
        GetAllVideoReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var translationStatusFilter = request.TranslationStatusFilter;
        var languageCount = await _repositoryWrapper.LocalizationLanguagesRepository.CountAsync();
        languageCount -= 1;

        Expression<Func<VideoReview, bool>> filter = vr =>
            vr.IsArchived == request.Archived &&
            (translationStatusFilter == null ||
                translationStatusFilter == TranslationStatusFilter.All ||
                (translationStatusFilter == TranslationStatusFilter.Outdated &&
                    vr.Localizations.Any(l => l.TranslationStatus == TranslationStatus.Outdated)) ||
                (translationStatusFilter == TranslationStatusFilter.Missing &&
                    vr.Localizations.Count < languageCount));

        var videoReviews = await _repositoryWrapper.VideoReviewsRepository.GetAllAsync(
            new QueryOptions<VideoReview>
            {
                Filter = filter,
                OrderByASC = videoReview => videoReview.Priority,
                AsNoTracking = true,
                Include = q => q.Include(x => x.Localizations).ThenInclude(l => l.Language)
            });

        return Result.Ok(_mapper.Map<List<VideoReviewDto>>(videoReviews));
    }
}
