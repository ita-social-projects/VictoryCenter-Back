using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgramCategories.Update;

public class UpdateHippotherapyProgramCategoryLocalizationHandler
    : IRequestHandler<UpdateHippotherapyProgramCategoryLocalizationCommand, Result<HippotherapyProgramCategoryLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyProgramCategory, HippotherapyProgramCategoryLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyProgramCategoryLocalizationCommand> _validator;

    public UpdateHippotherapyProgramCategoryLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyProgramCategory, HippotherapyProgramCategoryLocalization> localizationService,
        IValidator<UpdateHippotherapyProgramCategoryLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyProgramCategoryLocalizationDto>> Handle(
        UpdateHippotherapyProgramCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyProgramCategoryLocalization>(request.UpdateHippotherapyProgramCategoryLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyProgramCategoryLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyProgramCategoryLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyProgramCategoryLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgramCategoryLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyProgramCategoryLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyProgramCategoryLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyProgramCategoryLocalization)));
        }
    }
}
