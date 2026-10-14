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

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Create;

public class CreateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand, Result<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHippotherapyLandingPageScientificReferenceLocalizationUpdater _referencesUpdater;

    public CreateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> validator,
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
        CreateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto;

            var section = await GetSectionWithReferencesAsync(dto.EntityId);
            if (section is null)
            {
                return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                    ErrorMessagesConstants.NotFound(dto.EntityId, typeof(HippotherapyLandingPageScientificReferencesSectionEntity)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entity = _mapper.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(dto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);

            var referencesResult = await _referencesUpdater.UpsertReferencesAsync(section, dto.References, dto.LanguageId);
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
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
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
