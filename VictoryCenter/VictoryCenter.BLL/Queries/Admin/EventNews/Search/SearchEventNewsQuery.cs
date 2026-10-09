using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.EventNews.Search;

public record SearchEventNewsQuery(SearchEventNewsDto SearchEventNewsDto)
    : IRequest<Result<PaginationResult<EventNewsDto>>>;
