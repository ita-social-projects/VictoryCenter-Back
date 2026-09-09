using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

namespace VictoryCenter.BLL.Queries.Admin.Localization.VideoReviews.GetByEntityId;

public record GetVideoReviewLocalizationsByEntityIdQuery(long EntityId)
    : IRequest<Result<List<VideoReviewLocalizationDto>>>;
