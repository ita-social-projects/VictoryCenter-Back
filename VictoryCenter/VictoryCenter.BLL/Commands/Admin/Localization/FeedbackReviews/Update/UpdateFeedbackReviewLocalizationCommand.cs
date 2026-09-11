using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Update;

public record UpdateFeedbackReviewLocalizationCommand(
    long EntityId,
    long LanguageId,
    UpdateFeedbackReviewLocalizationDto Localization)
    : IValidatableRequest<Result<FeedbackReviewLocalizationDto>>;
