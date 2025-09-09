using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Programs;

namespace VictoryCenter.BLL.Commands.Admin.Programs.Update;

public record UpdateProgramCommand(UpdateProgramDto UpdateProgramDto, long Id) : IRequest<Result<ProgramDto>>;
