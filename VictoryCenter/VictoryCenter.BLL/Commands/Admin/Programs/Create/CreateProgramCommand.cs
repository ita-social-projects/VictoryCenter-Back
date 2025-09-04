using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Programs;

namespace VictoryCenter.BLL.Commands.Admin.Programs.Create;

public record CreateProgramCommand(CreateProgramDto CreateProgramDto) : IRequest<Result<ProgramDto>>;
