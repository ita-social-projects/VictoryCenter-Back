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

namespace VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Update;

public class UpdatePdfSectionLocalizationHandler
    : IRequestHandler<UpdatePdfSectionLocalizationCommand, Result<PdfSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePdfSectionLocalizationCommand> _validator;
    private readonly ILocalizationService<PdfSectionEntity, PdfSectionLocalization> _localizationService;

    public UpdatePdfSectionLocalizationHandler(
        IMapper mapper,
        IValidator<UpdatePdfSectionLocalizationCommand> validator,
        ILocalizationService<PdfSectionEntity, PdfSectionLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<PdfSectionLocalizationDto>> Handle(
        UpdatePdfSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.UpdatePdfSectionLocalizationDto;
            PdfSectionLocalization entity = _mapper.Map<PdfSectionLocalization>(dto);
            entity.LanguageId = request.LanguageId;

            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);

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
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(PdfSectionLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<PdfSectionLocalizationDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PdfSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PdfSectionLocalization)));
        }
    }
}
