using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Models;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.WhoWeAreSections;

public class WhoWeAreSectionsRepository : RepositoryBase<WhoWeAreSection>, IWhoWeAreSectionsRepository
{
    public WhoWeAreSectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public Task<List<WhoWeAreSectionPreviewRow>> GetPreviewRowsAsync(CancellationToken cancellationToken = default)
    {
        var textContents = DbContext.Set<WhoWeAreContent>().Where(content => content.ContentType != ContentType.Image);
        var statuses = textContents
            .SelectMany(content => content.Localizations, (content, localization) => new
            {
                content.SectionId,
                localization.LanguageId,
                localization.TranslationStatus
            })
            .GroupBy(localization => new { localization.SectionId, localization.LanguageId })
            .Where(group => group.Count() == textContents.Count(content => content.SectionId == group.Key.SectionId))
            .Select(group => new
            {
                group.Key.SectionId,
                group.Key.LanguageId,
                TranslationStatus = group.Count(localization => localization.TranslationStatus == TranslationStatus.Outdated) > 0
                    ? TranslationStatus.Outdated : TranslationStatus.Relevant
            });

        var previews =
            from section in DbContext.Set<WhoWeAreSection>().AsNoTracking()
            join status in statuses on section.Id equals status.SectionId into sectionStatuses
            from status in sectionStatuses.DefaultIfEmpty()
            orderby section.Id, status.LanguageId
            select new WhoWeAreSectionPreviewRow
            {
                Id = section.Id,
                SectionType = section.SectionType,
                Title = section.Title,
                LanguageId = (long?)status.LanguageId,
                TranslationStatus = (TranslationStatus?)status.TranslationStatus
            };

        return previews.ToListAsync(cancellationToken);
    }
}
