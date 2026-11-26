using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Errors;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetByLanguageId;

public class GetMainPageLocalizationByLanguageIdHandler
    : IRequestHandler<GetMainPageLocalizationByLanguageIdQuery, Result<MainPageLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetMainPageLocalizationByLanguageIdHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<MainPageLocalizationDto>> Handle(
        GetMainPageLocalizationByLanguageIdQuery request,
        CancellationToken cancellationToken)
    {
        var row = await _repositoryWrapper.MainPageRepository.GetLocalizationAsync(request.EntityId, request.LanguageId, cancellationToken);
        if (row is null)
        {
            return Result.Fail<MainPageLocalizationDto>(NotFoundErrors.NotFound(request.EntityId, typeof(MainPageEntity)));
        }

        if (row.Translation is not { } translation)
        {
            return Result.Fail<MainPageLocalizationDto>(NotFoundErrors.NotFound((request.EntityId, request.LanguageId), typeof(MainPageLocalization)));
        }

        return Result.Ok(new MainPageLocalizationDto
        {
            EntityId = translation.EntityId,
            Title = translation.Title,
            Description = translation.Description,
            TranslationStatus = translation.TranslationStatus,
            LocalizationInfoDto = new LocalizationInfoDto { Id = translation.LanguageId, Code = translation.LanguageCode },
            MainAboutUs = row.AboutUs is not { } about ? null : new MainAboutUsLocalizationDto
            {
                EntityId = about.EntityId, Title = about.Title, Description = about.Description, TranslationStatus = about.TranslationStatus,
                LocalizationInfoDto = new LocalizationInfoDto { Id = about.LanguageId, Code = about.LanguageCode }
            },
            MainPartners = row.Partners is not { } partners ? null : new MainPartnersLocalizationDto
            {
                EntityId = partners.EntityId, Title = partners.Title, Description = partners.Description, TranslationStatus = partners.TranslationStatus,
                LocalizationInfoDto = new LocalizationInfoDto { Id = partners.LanguageId, Code = partners.LanguageCode }
            },
            MainDonations = row.Donations is not { } donations ? null : new MainDonationsLocalizationDto
            {
                EntityId = donations.EntityId, Title = donations.Title, Description = donations.Description, TranslationStatus = donations.TranslationStatus,
                LocalizationInfoDto = new LocalizationInfoDto { Id = donations.LanguageId, Code = donations.LanguageCode }
            }
        });
    }
}
