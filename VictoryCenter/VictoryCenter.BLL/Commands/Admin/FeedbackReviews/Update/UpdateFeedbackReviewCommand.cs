using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Update;

public record UpdateFeedbackReviewCommand(long Id, UpdateFeedbackReviewDto FeedbackReview)
    : IValidatableRequest<Result<FeedbackReviewDto>>;
