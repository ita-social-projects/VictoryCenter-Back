using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Delete;

public class DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> _localizationService;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyLandingPageParticipantsSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization> localizationService,
        IRepositoryWrapper repositoryWrapper)
    {
        _localizationService = localizationService;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageParticipantsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repositoryWrapper.BeginTransaction();

            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);

            var cardIds = (await _repositoryWrapper.HippotherapyLandingPageParticipantCardsRepository
                .GetAllAsync(new QueryOptions<HippotherapyLandingPageParticipantCard>
                {
                    Filter = c => c.ParticipantsSectionId == request.EntityId
                }))
                .Select(c => c.Id)
                .ToList();

            if (cardIds.Count > 0)
            {
                await _repositoryWrapper.HippotherapyLandingPageParticipantCardLocalizationsRepository.BulkDeleteAsync(
                    l => l.LanguageId == request.LanguageId && cardIds.Contains(l.EntityId));
            }

            transaction.Complete();

            return Result.Ok(new DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageParticipantsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)));
        }
    }
}
