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

namespace VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Update;

public class UpdateReportFundsExpendituresCategoryLocalizationHandler
    : IRequestHandler<UpdateReportFundsExpendituresCategoryLocalizationCommand, Result<ReportFundsExpendituresCategoryLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization> _localizationService;
    private readonly IValidator<UpdateReportFundsExpendituresCategoryLocalizationCommand> _validator;

    public UpdateReportFundsExpendituresCategoryLocalizationHandler(
        IMapper mapper,
        ILocalizationService<ReportFundsExpendituresCategory, ReportFundsExpendituresCategoryLocalization> localizationService,
        IValidator<UpdateReportFundsExpendituresCategoryLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<ReportFundsExpendituresCategoryLocalizationDto>> Handle(
        UpdateReportFundsExpendituresCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<ReportFundsExpendituresCategoryLocalization>(request.UpdateReportFundsExpendituresCategoryLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
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
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresCategoryLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ReportFundsExpendituresCategoryLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresCategoryLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresCategoryLocalization)));
        }
    }
}
