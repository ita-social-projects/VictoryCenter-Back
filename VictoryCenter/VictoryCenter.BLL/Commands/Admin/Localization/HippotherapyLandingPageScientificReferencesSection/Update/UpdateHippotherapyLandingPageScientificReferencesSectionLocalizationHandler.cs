using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageScientificReferencesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Update;

public class UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler
    : IRequestHandler<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand, Result<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> _localizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHippotherapyLandingPageScientificReferenceLocalizationUpdater _referencesUpdater;

    public UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> localizationService,
        IValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IHippotherapyLandingPageScientificReferenceLocalizationUpdater referencesUpdater)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _referencesUpdater = referencesUpdater;
    }

    public async Task<Result<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>> Handle(
        UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto;

            var section = await GetSectionWithReferencesAsync(request.EntityId);
            if (section is null)
            {
                return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                    ErrorMessagesConstants.NotFound(request.EntityId, typeof(HippotherapyLandingPageScientificReferencesSectionEntity)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entity = _mapper.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(dto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);

            var referencesResult = await _referencesUpdater.UpsertReferencesAsync(section, dto.References, request.LanguageId);
            if (referencesResult.IsFailed)
            {
                return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(referencesResult.Errors);
            }

            transaction.Complete();

            var responseDto = _mapper.Map<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(result);
            responseDto.References = referencesResult.Value;
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
    }

    private Task<HippotherapyLandingPageScientificReferencesSectionEntity?> GetSectionWithReferencesAsync(long sectionId)
    {
        return _repositoryWrapper.HippotherapyLandingPageScientificReferencesSectionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyLandingPageScientificReferencesSectionEntity>
            {
                Filter = s => s.Id == sectionId,
                Include = q => q.Include(s => s.ScientificReferences)
            });
    }
}
