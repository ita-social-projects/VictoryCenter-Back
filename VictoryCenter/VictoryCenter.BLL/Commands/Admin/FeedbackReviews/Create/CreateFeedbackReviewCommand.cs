using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Create;

public record CreateFeedbackReviewCommand(CreateFeedbackReviewDto CreateFeedbackReviewDto)
    : IValidatableRequest<Result<FeedbackReviewDto>>;
