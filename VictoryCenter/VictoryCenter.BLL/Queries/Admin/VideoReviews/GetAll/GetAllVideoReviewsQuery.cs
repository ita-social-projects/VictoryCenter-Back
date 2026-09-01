using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;

namespace VictoryCenter.BLL.Queries.Admin.VideoReviews.GetAll;

public record GetAllVideoReviewsQuery : IRequest<Result<List<VideoReviewDto>>>;
