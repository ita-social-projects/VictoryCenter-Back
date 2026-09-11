using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Update;

public record UpdateFeedbackHistoryLocalizationCommand(
    long EntityId,
    long LanguageId,
    UpdateFeedbackHistoryLocalizationDto Localization)
    : IValidatableRequest<Result<FeedbackHistoryLocalizationDto>>;
