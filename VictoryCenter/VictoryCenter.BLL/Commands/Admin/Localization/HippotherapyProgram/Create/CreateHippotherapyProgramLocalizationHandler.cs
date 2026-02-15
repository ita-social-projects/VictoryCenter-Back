using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;


namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;

public class CreateHippotherapyProgramLocalizationHandler : IRequestHandler<CreateHippotherapyProgramLocalizationCommand, Result<HippotherapyProgramLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateHippotherapyProgramLocalizationDto> _validator;
    private readonly ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> _localizationService;

    public Task<Result<HippotherapyProgramLocalizationDto>> Handle(CreateHippotherapyProgramLocalizationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
