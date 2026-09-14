using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Reorder;

public record ReorderFeedbackHistoriesCommand(ReorderFeedbackHistoriesDto ReorderFeedbackHistoriesDto)
    : IRequest<Result<Unit>>;
