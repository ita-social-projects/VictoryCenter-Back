using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PdfSection.Update;

public class UpdatePdfSectionLocalizationHandler
    : IRequestHandler<UpdatePdfSectionLocalizationCommand, Result<PdfSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePdfSectionLocalizationCommand> _validator;
    private readonly ILocalizationService<PdfSectionEntity, PdfSectionLocalization> _localizationService;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHubContext<PdfReportsHub> _hubContext;

    public UpdatePdfSectionLocalizationHandler(
        IMapper mapper,
        IValidator<UpdatePdfSectionLocalizationCommand> validator,
        ILocalizationService<PdfSectionEntity, PdfSectionLocalization> localizationService,
        IRepositoryWrapper repositoryWrapper,
        IHubContext<PdfReportsHub> hubContext)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
        _repositoryWrapper = repositoryWrapper;
        _hubContext = hubContext;
    }

    public async Task<Result<PdfSectionLocalizationDto>> Handle(
        UpdatePdfSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var section = await _repositoryWrapper.PdfSectionRepository
                .GetFirstOrDefaultAsync(new QueryOptions<PdfSectionEntity>
                {
                    AsNoTracking = true,
                    Include = ps => ps.Include(s => s.Localizations).ThenInclude(l => l.Language),
                });

            if (section is null)
            {
                return Result.Fail<PdfSectionLocalizationDto>(
                    NotFoundErrors.NotFound());
            }

            var dto = request.UpdatePdfSectionLocalizationDto;
            PdfSectionLocalization entity = _mapper.Map<PdfSectionLocalization>(dto);
            entity.EntityId = section.Id;
            entity.LanguageId = request.LanguageId;

            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            await PdfSectionNotificationHelper.BroadcastUpdatedAsync(
                _hubContext,
                _mapper,
                section,
                result,
                cancellationToken);

            PdfSectionLocalizationDto responseDto = _mapper.Map<PdfSectionLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<PdfSectionLocalizationDto>(new NotFoundError(knfex.Message));
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<PdfSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(PdfSectionLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<PdfSectionLocalizationDto>(ex.Errors.Select(e => (IError)new ValidationError(e.PropertyName, e.ErrorMessage)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PdfSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PdfSectionLocalization)));
        }
    }
}
