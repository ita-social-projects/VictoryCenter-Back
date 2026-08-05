using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using PartnersPageBannerEntity = VictoryCenter.DAL.Entities.PartnersPageBanner;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Delete;

public class DeletePartnersPageBannerLocalizationHandler
    : IRequestHandler<DeletePartnersPageBannerLocalizationCommand, Result<DeletePartnersPageBannerLocalizationDto>>
{
    private readonly ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization> _localizationService;

    public DeletePartnersPageBannerLocalizationHandler(ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization> localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<DeletePartnersPageBannerLocalizationDto>> Handle(DeletePartnersPageBannerLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (entityId, languageId) = await _localizationService.DeleteEntityLocalizationAsync(request.EntityId, request.LanguageId);
            return Result.Ok(new DeletePartnersPageBannerLocalizationDto { EntityId = entityId, LanguageId = languageId });
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<DeletePartnersPageBannerLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(PartnersPageBannerLocalization)));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<DeletePartnersPageBannerLocalizationDto>(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(PartnersPageBannerLocalization)));
        }
    }
}
