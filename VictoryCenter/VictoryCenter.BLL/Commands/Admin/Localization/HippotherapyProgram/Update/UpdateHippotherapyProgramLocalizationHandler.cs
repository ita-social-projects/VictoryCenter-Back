using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Update;
public class UpdateHippotherapyProgramLocalizationHandler : IRequestHandler<UpdateHippotherapyProgramLocalizationCommand, Result<HippotherapyProgramLocalizationDto>>
{
    public Task<Result<HippotherapyProgramLocalizationDto>> Handle(UpdateHippotherapyProgramLocalizationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
