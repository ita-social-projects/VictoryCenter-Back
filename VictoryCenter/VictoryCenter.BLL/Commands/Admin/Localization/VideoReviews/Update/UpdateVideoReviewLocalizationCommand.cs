using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Update;

public record UpdateVideoReviewLocalizationCommand(
    long EntityId,
    long LanguageId,
    UpdateVideoReviewLocalizationDto Localization)
    : IValidatableRequest<Result<VideoReviewLocalizationDto>>;
