using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

public record GetPublishedEventNewsQuery(long? CategoryId, int? Offset, int? Limit)
    : IRequest<Result<PaginationResult<PublishedEventNewsDto>>>;
