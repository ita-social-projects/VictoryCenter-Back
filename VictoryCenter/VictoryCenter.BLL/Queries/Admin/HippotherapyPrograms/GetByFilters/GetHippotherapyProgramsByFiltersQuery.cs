using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetByFilters;

public record GetHippotherapyProgramsByFiltersQuery(HippotherapyProgramsFilterDto? RequestDto) : IRequest<Result<PaginationResult<HippotherapyProgramDto>>>;
