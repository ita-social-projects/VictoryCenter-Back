using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageQuoteSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageQuoteSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Update;

public class UpdateHippotherapyLandingPageQuoteSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand, Result<HippotherapyLandingPageQuoteSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand> _validator;

    public UpdateHippotherapyLandingPageQuoteSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageQuoteSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageQuoteSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageQuoteSectionLocalization>(
                request.UpdateHippotherapyLandingPageQuoteSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageQuoteSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageQuoteSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageQuoteSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageQuoteSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageQuoteSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageQuoteSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageQuoteSectionLocalization)));
        }
    }
}
