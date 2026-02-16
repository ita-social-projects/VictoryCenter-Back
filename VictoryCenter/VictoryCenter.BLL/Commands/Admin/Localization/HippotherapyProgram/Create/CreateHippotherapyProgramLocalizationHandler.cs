using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;

public class CreateHippotherapyProgramLocalizationHandler : IRequestHandler<CreateHippotherapyProgramLocalizationCommand, Result<HippotherapyProgramLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateHippotherapyProgramLocalizationCommand> _validator;
    private readonly ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> _localizationService;

    public CreateHippotherapyProgramLocalizationHandler(
        IMapper mapper,
        IValidator<CreateHippotherapyProgramLocalizationCommand> validator,
        ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<HippotherapyProgramLocalizationDto>> Handle(CreateHippotherapyProgramLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            HippotherapyProgramLocalization entity = _mapper.Map<HippotherapyProgramLocalization>(request.CreateHippotherapyProgramLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            HippotherapyProgramLocalizationDto responseDto = _mapper.Map<HippotherapyProgramLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyProgramLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(HippotherapyProgramLocalization)));
        }
    }
}
