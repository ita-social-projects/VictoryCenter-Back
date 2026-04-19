using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.ReportProgramExpendituresRecords.GetAll;

public class GetAllReportProgramExpendituresRecordsHandler
    : IRequestHandler<GetAllReportProgramExpendituresRecordsQuery,
        Result<IEnumerable<ReportProgramExpendituresRecordDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllReportProgramExpendituresRecordsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<ReportProgramExpendituresRecordDto>>> Handle(
        GetAllReportProgramExpendituresRecordsQuery request, CancellationToken cancellationToken)
    {
        var results = await _repositoryWrapper
            .ReportProgramExpendituresRecordsRepository
            .GetAllProjectedAsync(
                record => new ReportProgramExpendituresRecordDto
                {
                    Id = record.Id,
                    ReportingYear = record.ReportingYear,
                    HippotherapyProgramCategoryId = record.HippotherapyProgramCategoryId,
                    AmountUah = record.AmountUah,
                    AmountUsd = record.AmountUsd
                }, new QueryOptions<ReportProgramExpendituresRecord>
                {
                    Filter = record => request.HippotherapyProgramCategoryId == null ||
                                       record.HippotherapyProgramCategoryId == request.HippotherapyProgramCategoryId,
                    Limit = ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerOneRetrieval,
                    AsNoTracking = true
                });

        return Result.Ok(results);
    }
}
