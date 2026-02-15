using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;

public record CreateHippotherapyProgramLocalizationCommand(CreateHippotherapyProgramLocalizationDto CreateHippotherapyProgramLocalizationDto)
    : IRequest<Result<HippotherapyProgramLocalizationDto>>;
