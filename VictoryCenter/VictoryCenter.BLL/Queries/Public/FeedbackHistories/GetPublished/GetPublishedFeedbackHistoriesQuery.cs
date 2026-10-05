using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;

namespace VictoryCenter.BLL.Queries.Public.FeedbackHistories.GetPublished;

public record GetPublishedFeedbackHistoriesQuery : IRequest<Result<List<PublishedFeedbackHistoryDto>>>;
