using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;

namespace VictoryCenter.BLL.Queries.Public.FeedbackHistories.GetLatestPublished;

public record GetLatestPublishedFeedbackHistoriesQuery : IRequest<Result<List<PublishedFeedbackHistoryDto>>>;
