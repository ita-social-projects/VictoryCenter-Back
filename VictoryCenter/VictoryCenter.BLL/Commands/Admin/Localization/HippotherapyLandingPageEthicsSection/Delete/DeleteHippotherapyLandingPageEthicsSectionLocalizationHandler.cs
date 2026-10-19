using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageEthicsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Delete;

public class DeleteHippotherapyLandingPageEthicsSectionLocalizationHandler
    : IRequestHandler<DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand, Result<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>>
{
    private readonly ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization> _localizationService;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHippotherapyLandingPageEthicsSectionLocalizationHandler(
        ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization> localizationService,
        IRepositoryWrapper repositoryWrapper)
    {
        _localizationService = localizationService;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>> Handle(
        DeleteHippotherapyLandingPageEthicsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repositoryWrapper.BeginTransaction();

            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);

            var principleIds = (await _repositoryWrapper.HippotherapyLandingPageEthicsPrinciplesRepository
                .GetAllAsync(new QueryOptions<HippotherapyLandingPageEthicsPrinciple>
                {
                    Filter = r => r.EthicsSectionId == request.EntityId
                }))
                .Select(r => r.Id)
                .ToList();

            if (principleIds.Count > 0)
            {
                await _repositoryWrapper.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository.BulkDeleteAsync(
                    l => l.LanguageId == request.LanguageId && principleIds.Contains(l.EntityId));
            }

            transaction.Complete();

            return Result.Ok(new DeleteHippotherapyLandingPageEthicsSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyLandingPageEthicsSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeleteHippotherapyLandingPageEthicsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(HippotherapyLandingPageEthicsSectionLocalization)));
        }
    }
}
