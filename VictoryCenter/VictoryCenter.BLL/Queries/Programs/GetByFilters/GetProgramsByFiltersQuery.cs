using MediatR;
using FluentResults;
using VictoryCenter.BLL.DTOs.Programs;

namespace VictoryCenter.BLL.Queries.Programs.GetByFilters;

public record GetProgramsByFiltersQuery(ProgramFilterRequestDto? RequestDto) : IRequest<Result<ProgramsFilterResponseDto>>;
