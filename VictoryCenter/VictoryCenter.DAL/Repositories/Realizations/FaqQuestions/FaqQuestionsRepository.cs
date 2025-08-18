using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.FaqQuestions;

public class FaqQuestionsRepository : RepositoryBase<FaqQuestion>, IFaqQuestionsRepository
{
    public FaqQuestionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
