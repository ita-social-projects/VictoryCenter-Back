using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.Programs.GetByFilters;

public record GetProgramsByFiltersQuery(ProgramsFilterDto? RequestDto) : IRequest<Result<PaginationResult<ProgramDto>>>;
