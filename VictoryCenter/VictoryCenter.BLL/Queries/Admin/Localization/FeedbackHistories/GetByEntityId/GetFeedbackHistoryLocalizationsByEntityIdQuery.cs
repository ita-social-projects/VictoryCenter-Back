using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

namespace VictoryCenter.BLL.Queries.Admin.Localization.FeedbackHistories.GetByEntityId;

public record GetFeedbackHistoryLocalizationsByEntityIdQuery(long EntityId)
    : IRequest<Result<List<FeedbackHistoryLocalizationDto>>>;
