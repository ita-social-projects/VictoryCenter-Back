using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Create;

public class CreateReportFundsExpendituresSettingsLocalizationHandler
    : IRequestHandler<CreateReportFundsExpendituresSettingsLocalizationCommand, Result<ReportFundsExpendituresSettingsLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<ReportFundsExpendituresSettingsEntity, ReportFundsExpendituresSettingsLocalization> _localizationService;
    private readonly IValidator<CreateReportFundsExpendituresSettingsLocalizationCommand> _validator;

    public CreateReportFundsExpendituresSettingsLocalizationHandler(
        IMapper mapper,
        ILocalizationService<ReportFundsExpendituresSettingsEntity, ReportFundsExpendituresSettingsLocalization> localizationService,
        IValidator<CreateReportFundsExpendituresSettingsLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresSettingsLocalizationDto>> Handle(
        CreateReportFundsExpendituresSettingsLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<ReportFundsExpendituresSettingsLocalization>(request.CreateReportFundsExpendituresSettingsLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<ReportFundsExpendituresSettingsLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<ReportFundsExpendituresSettingsLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<ReportFundsExpendituresSettingsLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresSettingsLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ReportFundsExpendituresSettingsLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresSettingsLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresSettingsLocalization)));
        }
    }
}
