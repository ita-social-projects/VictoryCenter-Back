using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackReviews.Search;

public record SearchFeedbackReviewQuery(SearchFeedbackReviewDto SearchDto)
    : IRequest<Result<PaginationResult<FeedbackReviewDto>>>;