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

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageDescriptionSection.Create;

public class CreateHippotherapyLandingPageDescriptionSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand, Result<HippotherapyLandingPageDescriptionSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand> _validator;

    public CreateHippotherapyLandingPageDescriptionSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageDescriptionSectionEntity, HippotherapyLandingPageDescriptionSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageDescriptionSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageDescriptionSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageDescriptionSectionLocalization>(
                request.CreateHippotherapyLandingPageDescriptionSectionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
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
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageDescriptionSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageDescriptionSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageDescriptionSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageDescriptionSectionLocalization)));
        }
    }
}
