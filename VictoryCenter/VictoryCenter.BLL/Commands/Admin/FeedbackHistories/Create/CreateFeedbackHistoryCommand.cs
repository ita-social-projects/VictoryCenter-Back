using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Create;

public record CreateFeedbackHistoryCommand(CreateFeedbackHistoryDto CreateFeedbackHistoryDto)
    : IRequest<Result<FeedbackHistoryDto>>;
