using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Update;
public record UpdateHippotherapyProgramLocalizationCommand(UpdateHippotherapyProgramLocalizationDto UpdateHippotherapyProgramLocalizationDto, long EntityId,
    long LanguageId)
    : IRequest<Result<HippotherapyProgramLocalizationDto>>;
