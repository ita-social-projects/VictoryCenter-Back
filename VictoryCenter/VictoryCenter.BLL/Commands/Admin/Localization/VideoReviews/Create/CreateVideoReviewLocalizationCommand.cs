using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Create;

public record CreateVideoReviewLocalizationCommand(CreateVideoReviewLocalizationDto Localization)
    : IValidatableRequest<Result<VideoReviewLocalizationDto>>;
