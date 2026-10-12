using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageAdvantagesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Delete;

public class DeleteHippotherapyLandingPageAdvantagesSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization> _localizationService;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyLandingPageAdvantagesSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization> localizationService,
        IRepositoryWrapper repositoryWrapper)
    {
        _localizationService = localizationService;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageAdvantagesSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repositoryWrapper.BeginTransaction();

            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);

            var cardIds = (await _repositoryWrapper.HippotherapyLandingPageAdvantageCardsRepository
                .GetAllAsync(new QueryOptions<HippotherapyLandingPageAdvantageCard>
                {
                    Filter = c => c.AdvantagesSectionId == request.EntityId
                }))
                .Select(c => c.Id)
                .ToList();

            if (cardIds.Count > 0)
            {
                await _repositoryWrapper.HippotherapyLandingPageAdvantageCardLocalizationsRepository.BulkDeleteAsync(
                    l => l.LanguageId == request.LanguageId && cardIds.Contains(l.EntityId));
            }

            transaction.Complete();

            return Result.Ok(new DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageAdvantagesSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)));
        }
    }
}
