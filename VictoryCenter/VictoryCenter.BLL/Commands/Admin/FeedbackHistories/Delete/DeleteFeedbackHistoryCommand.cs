using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Delete;

public record DeleteFeedbackHistoryCommand(long Id)
    : IRequest<Result<long>>;
