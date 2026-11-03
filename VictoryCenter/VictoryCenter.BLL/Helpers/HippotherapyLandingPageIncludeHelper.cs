using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace VictoryCenter.BLL.Helpers;

public static class HippotherapyLandingPageIncludeHelper
{
    public static IIncludableQueryable<DAL.Entities.HippotherapyLandingPage, object> IncludeFullGraph(
        IQueryable<DAL.Entities.HippotherapyLandingPage> query)
    {
        return query
            .Include(e => e.IntroSection).ThenInclude(s => s!.Image)
            .Include(e => e.IntroSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.DescriptionSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.QuoteSection).ThenInclude(s => s!.Image)
            .Include(e => e.QuoteSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.HippoventionSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.HippoventionCenterSection).ThenInclude(s => s!.Image)
            .Include(e => e.HippoventionCenterSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.AdvantagesSection).ThenInclude(s => s!.AdvantageCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Image)
            .Include(e => e.AdvantagesSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.AdvantagesSection).ThenInclude(s => s!.AdvantageCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Localizations)
            .Include(e => e.AnalysisSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.ScientificReferencesSection).ThenInclude(s => s!.ScientificReferences.OrderBy(r => r.Priority))
            .Include(e => e.ScientificReferencesSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.ScientificReferencesSection).ThenInclude(s => s!.ScientificReferences.OrderBy(r => r.Priority)).ThenInclude(r => r.Localizations)
            .Include(e => e.AnotherQuoteSection).ThenInclude(s => s!.Image)
            .Include(e => e.AnotherQuoteSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.ParticipantsSection).ThenInclude(s => s!.ParticipantCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Image)
            .Include(e => e.ParticipantsSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.ParticipantsSection).ThenInclude(s => s!.ParticipantCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Localizations)
            .Include(e => e.EthicsSection).ThenInclude(s => s!.Image)
            .Include(e => e.EthicsSection).ThenInclude(s => s!.EthicsPrinciples.OrderBy(p => p.Priority))
            .Include(e => e.EthicsSection).ThenInclude(s => s!.Localizations).ThenInclude(l => l.Language)
            .Include(e => e.EthicsSection).ThenInclude(s => s!.EthicsPrinciples.OrderBy(p => p.Priority)).ThenInclude(p => p.Localizations);
    }

    public static bool HasAllSections(this DAL.Entities.HippotherapyLandingPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        object?[] sections =
        [
            page.IntroSection,
            page.DescriptionSection,
            page.QuoteSection,
            page.HippoventionSection,
            page.HippoventionCenterSection,
            page.AdvantagesSection,
            page.AnalysisSection,
            page.ScientificReferencesSection,
            page.AnotherQuoteSection,
            page.ParticipantsSection,
            page.EthicsSection,
        ];

        return Array.TrueForAll(sections, static s => s is not null);
    }
}
