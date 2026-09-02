using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Delete;

public record DeleteFeedbackReviewCommand(long Id) : IRequest<Result<long>>;
