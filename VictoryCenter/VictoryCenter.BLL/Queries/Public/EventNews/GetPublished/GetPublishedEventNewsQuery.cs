using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Public.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

public record GetPublishedEventNewsQuery(int? Take = null) : IValidatableRequest<Result<List<PublishedEventNewsDto>>>;
