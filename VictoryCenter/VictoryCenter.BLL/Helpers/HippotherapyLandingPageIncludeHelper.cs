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
            .Include(e => e.DescriptionSection)
            .Include(e => e.QuoteSection).ThenInclude(s => s!.Image)
            .Include(e => e.HippoventionSection)
            .Include(e => e.HippoventionCenterSection).ThenInclude(s => s!.Image)
            .Include(e => e.AdvantagesSection).ThenInclude(s => s!.AdvantageCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Image)
            .Include(e => e.AnalysisSection)
            .Include(e => e.ScientificReferencesSection).ThenInclude(s => s!.ScientificReferences.OrderBy(r => r.Priority))
            .Include(e => e.AnotherQuoteSection).ThenInclude(s => s!.Image)
            .Include(e => e.ParticipantsSection).ThenInclude(s => s!.ParticipantCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Image)
            .Include(e => e.EthicsSection).ThenInclude(s => s!.Image)
            .Include(e => e.EthicsSection).ThenInclude(s => s!.EthicsPrinciples.OrderBy(p => p.Priority));
    }
}
