using MediatR;
using FluentResults;
using VictoryCenter.BLL.DTOs.Programs;

namespace VictoryCenter.BLL.Queries.Programs.GetById;

public record GetProgramByIdQuery(long id) : IRequest<Result<ProgramDto>>;
