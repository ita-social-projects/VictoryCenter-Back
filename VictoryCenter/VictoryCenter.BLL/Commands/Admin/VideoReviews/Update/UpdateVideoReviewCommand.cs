using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Update;

public record UpdateVideoReviewCommand(long Id, UpdateVideoReviewDto VideoReview)
    : IValidatableRequest<Result<VideoReviewDto>>;
