using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using HippotherapyLandingPageAnalysisSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAnalysisSection.Create;

public class CreateHippotherapyLandingPageAnalysisSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand, Result<HippotherapyLandingPageAnalysisSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand> _validator;

    public CreateHippotherapyLandingPageAnalysisSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageAnalysisSectionEntity, HippotherapyLandingPageAnalysisSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<HippotherapyLandingPageAnalysisSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageAnalysisSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var entity = _mapper.Map<HippotherapyLandingPageAnalysisSectionLocalization>(
                request.CreateHippotherapyLandingPageAnalysisSectionLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            var responseDto = _mapper.Map<HippotherapyLandingPageAnalysisSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageAnalysisSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageAnalysisSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageAnalysisSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageAnalysisSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageAnalysisSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageAnalysisSectionLocalization)));
        }
    }
}
