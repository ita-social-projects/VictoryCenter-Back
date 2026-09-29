using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.FeedbackReviews;

namespace VictoryCenter.BLL.Queries.Public.FeedbackReviews.GetPublished;

public record GetPublishedFeedbackReviewsQuery : IRequest<Result<List<PublishedFeedbackReviewDto>>>;
