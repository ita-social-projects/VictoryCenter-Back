using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.EventNews;

namespace VictoryCenter.BLL.Queries.Admin.EventNews.GetById;

public record GetEventNewsByIdQuery(long Id) : IRequest<Result<EventNewsDto>>;
