using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Delete;

public class DeleteReportFundsExpendituresCategoryHandler
    : IRequestHandler<DeleteReportFundsExpendituresCategoryCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteReportFundsExpendituresCategoryHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteReportFundsExpendituresCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryToDelete = await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresCategory>
            {
                Filter = category => category.Id == request.Id,
                Include = category => category.Include(c => c.Records)
            });

        if (categoryToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants.NotFound(request.Id, typeof(ReportFundsExpendituresCategory)));
        }

        if (categoryToDelete.Records.Count > 0)
        {
            return Result.Fail<long>(ReportFundsExpendituresCategoryConstants.CantDeleteCategoryWhileAssociatedWithAnyRecord);
        }

        if (ReportFundsExpendituresCategoryValidationHelper.IsReservedCategoryName(categoryToDelete.Name, categoryToDelete.Type))
        {
            return Result.Fail<long>(ReportFundsExpendituresCategoryConstants.CantDeleteReservedCategory);
        }

        _repositoryWrapper.ReportFundsExpendituresCategoriesRepository.Delete(categoryToDelete);

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(categoryToDelete.Id);
            }
        }
        catch (DbUpdateException)
        {
            return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresCategory)));
        }

        return Result.Fail<long>(ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresCategory)));
    }
}
