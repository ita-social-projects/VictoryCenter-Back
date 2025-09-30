using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;

namespace VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetById;

public record GetProgramByIdQuery(long Id) : IRequest<Result<HypotherapyProgramDto>>;
