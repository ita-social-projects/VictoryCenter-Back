using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;

public record CreateHippotherapyProgramCommand(CreateHippotherapyProgramDto CreateProgramDto) : IRequest<Result<HippotherapyProgramDto>>;
