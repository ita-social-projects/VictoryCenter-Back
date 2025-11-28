using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.FaqQuestions;

public class FaqQuestionLocalizationsRepository : RepositoryBase<FaqQuestionLocalization>, IFaqQuestionLocalizationsRepository
{
    public FaqQuestionLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
