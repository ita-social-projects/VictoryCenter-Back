using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Programs;

namespace VictoryCenter.BLL.Queries.Admin.Programs.GetById;

public record GetProgramByIdQuery(long Id) : IRequest<Result<ProgramDto>>;
