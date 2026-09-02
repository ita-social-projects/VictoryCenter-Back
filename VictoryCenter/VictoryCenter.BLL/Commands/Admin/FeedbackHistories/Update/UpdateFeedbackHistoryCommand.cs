using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Update;

public record UpdateFeedbackHistoryCommand(UpdateFeedbackHistoryDto UpdateFeedbackHistoryDto, long Id)
    : IRequest<Result<FeedbackHistoryDto>>;
