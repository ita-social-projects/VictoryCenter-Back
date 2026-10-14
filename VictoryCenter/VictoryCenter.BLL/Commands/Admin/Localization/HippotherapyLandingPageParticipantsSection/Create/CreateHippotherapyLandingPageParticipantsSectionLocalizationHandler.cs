using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Create;

public class CreateHippotherapyLandingPageParticipantsSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand, Result<HippotherapyLandingPageParticipantsSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand> _validator;

    public CreateHippotherapyLandingPageParticipantsSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageParticipantsSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageParticipantsSectionLocalization>(
                request.CreateHippotherapyLandingPageParticipantsSectionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageParticipantsSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
    }
}
