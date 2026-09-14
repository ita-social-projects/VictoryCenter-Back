using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Delete;

public record DeleteFeedbackReviewLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteFeedbackReviewLocalizationDto>>;
