using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetById;

public record GetHippotherapyProgramByIdQuery(long Id) : IRequest<Result<HippotherapyProgramDto>>;
