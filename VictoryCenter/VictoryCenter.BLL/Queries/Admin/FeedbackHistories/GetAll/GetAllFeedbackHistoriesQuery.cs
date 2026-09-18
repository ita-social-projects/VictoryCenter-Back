using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Enums;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackHistories.GetAll;

public record GetAllFeedbackHistoriesQuery(TranslationStatusFilter? TranslationStatusFilter = null)
    : IRequest<Result<IEnumerable<FeedbackHistoryDto>>>;
