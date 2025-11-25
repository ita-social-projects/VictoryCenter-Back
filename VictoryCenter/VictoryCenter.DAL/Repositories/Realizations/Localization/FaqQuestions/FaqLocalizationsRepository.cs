using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.FaqQuestions;

public class FaqLocalizationsRepository : RepositoryBase<FaqQuestionLocalization>, IFaqLocalizationsRepository
{
    public FaqLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
