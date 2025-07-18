using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPageQuestions;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.FaqPageQuestions;

public class FaqPageQuestionsRepository : RepositoryBase<FaqPageQuestion>, IFaqPageQuestionsRepository
{
    public FaqPageQuestionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
