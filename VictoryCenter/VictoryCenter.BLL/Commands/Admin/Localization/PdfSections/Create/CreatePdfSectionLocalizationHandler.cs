using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PdfSections.Create;

public class CreatePdfSectionLocalizationHandler
    : IRequestHandler<CreatePdfSectionLocalizationCommand, Result<PdfSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePdfSectionLocalizationCommand> _validator;
    private readonly ILocalizationService<PdfSectionEntity, PdfSectionLocalization> _localizationService;

    public CreatePdfSectionLocalizationHandler(
        IMapper mapper,
        IValidator<CreatePdfSectionLocalizationCommand> validator,
        ILocalizationService<PdfSectionEntity, PdfSectionLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<PdfSectionLocalizationDto>> Handle(
        CreatePdfSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            PdfSectionLocalization entity = _mapper.Map<PdfSectionLocalization>(request.Dto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);

            PdfSectionLocalizationDto responseDto = _mapper.Map<PdfSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<PdfSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<PdfSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(PdfSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PdfSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PdfSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PdfSectionLocalization)));
        }
    }
}
