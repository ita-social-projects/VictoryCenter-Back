using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.VideoReviews;

namespace VictoryCenter.BLL.Queries.Public.VideoReviews.GetPublished;

public record GetPublishedVideoReviewsQuery : IRequest<Result<List<PublishedVideoReviewDto>>>;
