using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;

public record UpdateHippotherapyProgramCommand(HippotherapyUpdateProgramDto UpdateProgramDto, long Id) : IRequest<Result<HippotherapyProgramDto>>;
