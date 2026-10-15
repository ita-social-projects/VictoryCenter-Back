using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageScientificReferencesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Delete;

public class DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> _localizationService;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization> localizationService,
        IRepositoryWrapper repositoryWrapper)
    {
        _localizationService = localizationService;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repositoryWrapper.BeginTransaction();

            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);

            var referenceIds = (await _repositoryWrapper.HippotherapyLandingPageScientificReferencesRepository
                .GetAllAsync(new QueryOptions<HippotherapyLandingPageScientificReference>
                {
                    Filter = r => r.ScientificReferencesSectionId == request.EntityId
                }))
                .Select(r => r.Id)
                .ToList();

            if (referenceIds.Count > 0)
            {
                await _repositoryWrapper.HippotherapyLandingPageScientificReferenceLocalizationsRepository.BulkDeleteAsync(
                    l => l.LanguageId == request.LanguageId && referenceIds.Contains(l.EntityId));
            }

            transaction.Complete();

            return Result.Ok(new DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)));
        }
    }
}
