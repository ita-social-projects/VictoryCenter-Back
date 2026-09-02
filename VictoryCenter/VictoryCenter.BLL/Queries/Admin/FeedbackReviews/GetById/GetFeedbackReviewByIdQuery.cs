using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetById;

public record GetFeedbackReviewByIdQuery(long Id) : IRequest<Result<FeedbackReviewDto>>;
