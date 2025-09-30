using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Update;

public record UpdateHypotherapyProgramCommand(HypotherapyUpdateProgramDto UpdateProgramDto, long Id) : IRequest<Result<HypotherapyProgramDto>>;
