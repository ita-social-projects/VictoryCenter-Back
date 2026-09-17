using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.FeedbackHistories.Search;

public record SearchFeedbackHistoryQuery(SearchFeedbackHistoryDto SearchDto)
    : IRequest<Result<PaginationResult<FeedbackHistoryDto>>>;