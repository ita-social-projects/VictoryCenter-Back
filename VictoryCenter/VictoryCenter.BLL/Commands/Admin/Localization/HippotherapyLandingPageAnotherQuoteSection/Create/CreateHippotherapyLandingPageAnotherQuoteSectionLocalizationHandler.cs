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

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.Create;

public class CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand, Result<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand> _validator;

    public CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageAnotherQuoteSectionEntity, HippotherapyLandingPageAnotherQuoteSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageAnotherQuoteSectionLocalization>(
                request.CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
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
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageAnotherQuoteSectionLocalization)));
        }
    }
}
