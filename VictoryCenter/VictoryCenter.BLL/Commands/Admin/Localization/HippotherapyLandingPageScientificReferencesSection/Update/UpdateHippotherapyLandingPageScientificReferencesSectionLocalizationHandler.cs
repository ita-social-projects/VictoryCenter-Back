using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageScientificReferencesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Update;

public class UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand, Result<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> _validator;

    public UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(
                request.UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
    }
}
