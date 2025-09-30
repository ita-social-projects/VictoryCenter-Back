using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Create;

public record CreateHypotherapyProgramCommand(CreateHypotherapyProgramDto CreateProgramDto) : IRequest<Result<HypotherapyProgramDto>>;
