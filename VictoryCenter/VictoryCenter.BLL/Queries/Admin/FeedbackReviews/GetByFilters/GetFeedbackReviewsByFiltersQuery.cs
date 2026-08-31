using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetByFilters;

public record GetFeedbackReviewsByFiltersQuery(FeedbackReviewsFilterDto Filter)
    : IValidatableRequest<Result<PaginationResult<FeedbackReviewDto>>>;
