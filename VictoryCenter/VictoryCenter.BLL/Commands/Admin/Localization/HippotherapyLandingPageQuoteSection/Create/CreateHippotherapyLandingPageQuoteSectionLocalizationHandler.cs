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

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageQuoteSection.Create;

public class CreateHippotherapyLandingPageQuoteSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageQuoteSectionLocalizationCommand, Result<HippotherapyLandingPageQuoteSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageQuoteSectionLocalizationCommand> _validator;

    public CreateHippotherapyLandingPageQuoteSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageQuoteSectionEntity, HippotherapyLandingPageQuoteSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageQuoteSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageQuoteSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageQuoteSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageQuoteSectionLocalization>(
                request.CreateHippotherapyLandingPageQuoteSectionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
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
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageQuoteSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageQuoteSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageQuoteSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageQuoteSectionLocalization)));
        }
    }
}
