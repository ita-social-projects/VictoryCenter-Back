using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageIntroSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageIntroSection.Create;

public class CreateHippotherapyLandingPageIntroSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageIntroSectionLocalizationCommand, Result<HippotherapyLandingPageIntroSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageIntroSectionEntity, HippotherapyLandingPageIntroSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageIntroSectionLocalizationCommand> _validator;

    public CreateHippotherapyLandingPageIntroSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageIntroSectionEntity, HippotherapyLandingPageIntroSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageIntroSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageIntroSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageIntroSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageIntroSectionLocalization>(
                request.CreateHippotherapyLandingPageIntroSectionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageIntroSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageIntroSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageIntroSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageIntroSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageIntroSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageIntroSectionLocalization)));
        }
    }
}
