using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.Search;

public record SearchHippotherapyProgramsQuery(SearchHippotherapyProgramDto SearchHippotherapyProgramDto)
    : IRequest<Result<PaginationResult<HippotherapyProgramDto>>>;
