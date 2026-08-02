using FluentResults;
using VictoryCenter.BLL.Behaviors.Abstractions;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.EventNews.GetByFilters;

public record GetEventNewsByFiltersQuery(EventNewsFilterDto Filter)
    : IValidatableRequest<Result<PaginationResult<EventNewsDto>>>;
