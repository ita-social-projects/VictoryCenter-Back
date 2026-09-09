using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

namespace VictoryCenter.BLL.Queries.Admin.Localization.FeedbackReviews.GetByEntityId;

public record GetFeedbackReviewLocalizationsByEntityIdQuery(long EntityId)
    : IRequest<Result<List<FeedbackReviewLocalizationDto>>>;
