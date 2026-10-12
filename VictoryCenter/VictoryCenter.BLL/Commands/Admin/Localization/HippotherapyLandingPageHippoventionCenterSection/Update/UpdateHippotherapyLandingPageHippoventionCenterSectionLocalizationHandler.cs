using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageHippoventionCenterSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageHippoventionCenterSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.Update;

public class UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand, Result<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand> _validator;

    public UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageHippoventionCenterSectionEntity, HippotherapyLandingPageHippoventionCenterSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageHippoventionCenterSectionLocalization>(
                request.UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageHippoventionCenterSectionLocalization)));
        }
    }
}
