using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;

namespace VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetById;

public record GetHypotherapyProgramByIdQuery(long Id) : IRequest<Result<HypotherapyProgramDto>>;
