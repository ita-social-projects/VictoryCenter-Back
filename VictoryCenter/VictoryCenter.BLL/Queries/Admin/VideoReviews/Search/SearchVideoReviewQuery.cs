using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.VideoReviews.Search;

public record SearchVideoReviewQuery(SearchVideoReviewDto SearchDto)
    : IRequest<Result<PaginationResult<VideoReviewDto>>>;