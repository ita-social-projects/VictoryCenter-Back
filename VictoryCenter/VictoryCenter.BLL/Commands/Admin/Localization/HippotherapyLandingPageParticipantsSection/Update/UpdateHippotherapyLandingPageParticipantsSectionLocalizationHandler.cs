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

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Update;

public class UpdateHippotherapyLandingPageParticipantsSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand, Result<HippotherapyLandingPageParticipantsSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand> _validator;

    public UpdateHippotherapyLandingPageParticipantsSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageParticipantsSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageParticipantsSectionLocalization>(
                request.UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
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
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
    }
}
