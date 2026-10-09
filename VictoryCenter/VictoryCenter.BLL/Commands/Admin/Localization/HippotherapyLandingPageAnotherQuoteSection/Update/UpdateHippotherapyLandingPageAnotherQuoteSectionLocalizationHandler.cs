using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAnotherQuoteSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAnotherQuoteSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Update;

public class UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand, Result<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand> _validator;

    public UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(
                request.UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)));
        }
    }
}
