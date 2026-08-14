using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Delete;

public class DeletePartnerSectionLocalizationHandler
    : IRequestHandler<DeletePartnerSectionLocalizationCommand, Result<DeletePartnerSectionLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<PartnerSection, PartnerSectionLocalization> _sectionLocalizationService;

    public DeletePartnerSectionLocalizationHandler(
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<PartnerSection, PartnerSectionLocalization> sectionLocalizationService)
    {
        _repositoryWrapper = repositoryWrapper;
        _sectionLocalizationService = sectionLocalizationService;
    }

    public async Task<Result<DeletePartnerSectionLocalizationDto>> Handle(DeletePartnerSectionLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repositoryWrapper.BeginTransaction();

            var (entityId, languageId) = await _sectionLocalizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);

            var partnerIds = (await _repositoryWrapper.PartnerRepository
                .GetAllAsync(new QueryOptions<Partner>
                {
                    Filter = p => p.PartnersSectionId == request.EntityId
                }))
                .Select(p => p.Id)
                .ToList();

            if (partnerIds.Count > 0)
            {
                await _repositoryWrapper.PartnerLocalizationsRepository.BulkDeleteAsync(
                    l => l.LanguageId == request.LanguageId && partnerIds.Contains(l.EntityId));
            }

            transaction.Complete();

            return Result.Ok(new DeletePartnerSectionLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeletePartnerSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnerSectionLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeletePartnerSectionLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(PartnerSectionLocalization)));
        }
    }
}
