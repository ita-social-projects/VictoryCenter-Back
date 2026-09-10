using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Reorder;

public record ReorderFeedbackReviewsCommand(ReorderFeedbackReviewsDto ReorderFeedbackReviewsDto)
    : IRequest<Result<Unit>>;
