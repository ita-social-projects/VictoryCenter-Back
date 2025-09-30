using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetByFilters;

public record GetProgramsByFiltersQuery(HypotherapyProgramsFilterDto? RequestDto) : IRequest<Result<PaginationResult<HypotherapyProgramDto>>>;
