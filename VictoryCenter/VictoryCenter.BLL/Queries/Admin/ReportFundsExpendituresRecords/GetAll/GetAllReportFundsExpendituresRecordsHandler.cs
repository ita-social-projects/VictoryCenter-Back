using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetAll;

public class GetAllReportFundsExpendituresRecordsHandler
    : IRequestHandler<GetAllReportFundsExpendituresRecordsQuery, Result<IEnumerable<ReportFundsExpendituresRecordDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllReportFundsExpendituresRecordsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<ReportFundsExpendituresRecordDto>>> Handle(
        GetAllReportFundsExpendituresRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var records = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository.GetAllAsync();
        return Result.Ok(_mapper.Map<IEnumerable<ReportFundsExpendituresRecordDto>>(records));
    }
}
