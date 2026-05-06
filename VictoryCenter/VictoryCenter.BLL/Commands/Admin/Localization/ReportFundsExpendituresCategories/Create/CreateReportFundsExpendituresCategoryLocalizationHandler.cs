using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Create;

public class CreateReportFundsExpendituresCategoryLocalizationHandler
    : IRequestHandler<CreateReportFundsExpendituresCategoryLocalizationCommand, Result<ReportFundsExpendituresCategoryLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization> _localizationService;
    private readonly IValidator<CreateReportFundsExpendituresCategoryLocalizationCommand> _validator;

    public CreateReportFundsExpendituresCategoryLocalizationHandler(
        IMapper mapper,
        ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization> localizationService,
        IValidator<CreateReportFundsExpendituresCategoryLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresCategoryLocalizationDto>> Handle(
        CreateReportFundsExpendituresCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<ReportFundsExpendituresCategoryLocalization>(request.CreateReportFundsExpendituresCategoryLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<ReportFundsExpendituresCategoryLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<ReportFundsExpendituresCategoryLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<ReportFundsExpendituresCategoryLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresCategoryLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ReportFundsExpendituresCategoryLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresCategoryLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresCategoryLocalization)));
        }
    }
}
