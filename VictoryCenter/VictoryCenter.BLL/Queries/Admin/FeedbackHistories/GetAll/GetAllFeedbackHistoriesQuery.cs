using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackHistories.GetAll;

public record GetAllFeedbackHistoriesQuery
    : IRequest<Result<IEnumerable<FeedbackHistoryDto>>>;
