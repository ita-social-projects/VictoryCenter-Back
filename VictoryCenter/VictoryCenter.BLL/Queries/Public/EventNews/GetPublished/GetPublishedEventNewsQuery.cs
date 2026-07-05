using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

public record GetPublishedEventNewsQuery(int? Take = null) : IRequest<Result<List<PublishedEventNewsDto>>>;
