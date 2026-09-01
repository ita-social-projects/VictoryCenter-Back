using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Create;

public record CreateVideoReviewCommand(CreateVideoReviewDto VideoReview)
    : IValidatableRequest<Result<VideoReviewDto>>;
