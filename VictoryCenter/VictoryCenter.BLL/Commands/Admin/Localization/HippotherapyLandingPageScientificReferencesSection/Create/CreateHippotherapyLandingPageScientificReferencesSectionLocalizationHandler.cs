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

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Create;

public class CreateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand, Result<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> _validator;

    public CreateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(
                request.CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
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
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
    }
}
