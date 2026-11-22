using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Models;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.DAL.Repositories.Realizations.MainPage;

public class MainPageRepository : RepositoryBase<MainPageEntity>, IMainPageRepository
{
    public MainPageRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public Task<MainPageLocalizationRow?> GetLocalizationAsync(long entityId, long languageId, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<MainPageEntity>()
            .AsNoTracking()
            .Where(page => page.Id == entityId)
            .LeftJoin(
                DbContext.Set<MainPageLocalization>().Where(localization => localization.LanguageId == languageId),
                page => page.Id,
                localization => localization.EntityId,
                (page, translation) => new { PageId = page.Id, Translation = translation })
            .LeftJoin(
                DbContext.Set<MainAboutUsLocalization>().Where(localization => localization.LanguageId == languageId),
                row => row.PageId,
                localization => localization.Entity.MainPageId,
                (row, about) => new { row.PageId, row.Translation, AboutUs = about })
            .LeftJoin(
                DbContext.Set<MainPartnersLocalization>().Where(localization => localization.LanguageId == languageId),
                row => row.PageId,
                localization => localization.Entity.MainPageId,
                (row, partners) => new { row.PageId, row.Translation, row.AboutUs, Partners = partners })
            .LeftJoin(
                DbContext.Set<MainDonationsLocalization>().Where(localization => localization.LanguageId == languageId),
                row => row.PageId,
                localization => localization.Entity.MainPageId,
                (row, donations) => new { row.Translation, row.AboutUs, row.Partners, Donations = donations })
            .Select(row => new MainPageLocalizationRow
            {
                Translation = row.Translation == null ? null : new MainPageTranslationRow
                {
                    EntityId = row.Translation.EntityId,
                    LanguageId = row.Translation.LanguageId,
                    LanguageCode = row.Translation.Language.Code,
                    Title = row.Translation.Title,
                    Description = row.Translation.Description,
                    TranslationStatus = row.Translation.TranslationStatus
                },
                AboutUs = row.AboutUs == null ? null : new MainPageTranslationRow
                {
                    EntityId = row.AboutUs.EntityId,
                    LanguageId = row.AboutUs.LanguageId,
                    LanguageCode = row.AboutUs.Language.Code,
                    Title = row.AboutUs.Title,
                    Description = row.AboutUs.Description,
                    TranslationStatus = row.AboutUs.TranslationStatus
                },
                Partners = row.Partners == null ? null : new MainPageTranslationRow
                {
                    EntityId = row.Partners.EntityId,
                    LanguageId = row.Partners.LanguageId,
                    LanguageCode = row.Partners.Language.Code,
                    Title = row.Partners.Title,
                    Description = row.Partners.Description,
                    TranslationStatus = row.Partners.TranslationStatus
                },
                Donations = row.Donations == null ? null : new MainPageTranslationRow
                {
                    EntityId = row.Donations.EntityId,
                    LanguageId = row.Donations.LanguageId,
                    LanguageCode = row.Donations.Language.Code,
                    Title = row.Donations.Title,
                    Description = row.Donations.Description,
                    TranslationStatus = row.Donations.TranslationStatus
                }
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
