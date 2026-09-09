using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Create;

public record CreateFeedbackHistoryLocalizationCommand(CreateFeedbackHistoryLocalizationDto Localization)
    : IValidatableRequest<Result<FeedbackHistoryLocalizationDto>>;
