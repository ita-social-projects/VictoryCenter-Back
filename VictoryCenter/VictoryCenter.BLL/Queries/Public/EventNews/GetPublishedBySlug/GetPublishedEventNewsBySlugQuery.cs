using FluentResults;
using MediatR;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Public.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublishedBySlug;

public record GetPublishedEventNewsBySlugQuery(string Slug)
    : IRequest<Result<PublishedEventNewsDetailsDto>>, IBaseValidatableRequest;
