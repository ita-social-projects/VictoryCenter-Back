using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublishedBySlug;

public record GetPublishedEventNewsBySlugQuery(string Slug)
    : IRequest<Result<PublishedEventNewsDetailsDto>>;
