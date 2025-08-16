using MediatR;
using FluentResults;
using VictoryCenter.BLL.DTOs.Programs;

namespace VictoryCenter.BLL.Commands.Programs.Create;

public record CreateProgramCommand(CreateProgramDto createProgramDto) : IRequest<Result<ProgramDto>>;
