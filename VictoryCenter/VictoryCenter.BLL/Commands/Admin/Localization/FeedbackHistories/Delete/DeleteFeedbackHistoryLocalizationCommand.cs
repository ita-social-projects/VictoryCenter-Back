using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Delete;

public record DeleteFeedbackHistoryLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteFeedbackHistoryLocalizationDto>>;
