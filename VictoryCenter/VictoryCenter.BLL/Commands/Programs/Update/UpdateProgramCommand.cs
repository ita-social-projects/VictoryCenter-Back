using MediatR;
using FluentResults;
using VictoryCenter.BLL.DTOs.Programs;

namespace VictoryCenter.BLL.Commands.Programs.Update;

public record UpdateProgramCommand(UpdateProgramDto updateProgramDto) : IRequest<Result<ProgramDto>>;
