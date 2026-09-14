using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Create;

public record CreateFeedbackReviewLocalizationCommand(CreateFeedbackReviewLocalizationDto Localization)
    : IValidatableRequest<Result<FeedbackReviewLocalizationDto>>;
