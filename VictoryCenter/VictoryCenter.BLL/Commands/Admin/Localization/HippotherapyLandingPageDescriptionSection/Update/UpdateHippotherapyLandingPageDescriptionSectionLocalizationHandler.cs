using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageDescriptionSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Update;

public class UpdateHippotherapyLandingPageDescriptionSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageDescriptionSectionLocalizationCommand, Result<HippotherapyLandingPageDescriptionSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageDescriptionSectionLocalizationCommand> _validator;

    public UpdateHippotherapyLandingPageDescriptionSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageDescriptionSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageDescriptionSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageDescriptionSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageDescriptionSectionLocalization>(
                request.UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageDescriptionSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageDescriptionSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageDescriptionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageDescriptionSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageDescriptionSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageDescriptionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageDescriptionSectionLocalization)));
        }
    }
}
