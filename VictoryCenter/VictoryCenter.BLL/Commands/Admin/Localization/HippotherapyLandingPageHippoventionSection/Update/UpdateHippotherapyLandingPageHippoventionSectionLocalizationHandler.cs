using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageHippoventionSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageHippoventionSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionSection.Update;

public class UpdateHippotherapyLandingPageHippoventionSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageHippoventionSectionLocalizationCommand, Result<HippotherapyLandingPageHippoventionSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageHippoventionSectionEntity, HippotherapyLandingPageHippoventionSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageHippoventionSectionLocalizationCommand> _validator;

    public UpdateHippotherapyLandingPageHippoventionSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageHippoventionSectionEntity, HippotherapyLandingPageHippoventionSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageHippoventionSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageHippoventionSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageHippoventionSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageHippoventionSectionLocalization>(
                request.UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageHippoventionSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageHippoventionSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageHippoventionSectionLocalization)));
        }
    }
}
