using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresSettings.Get;

public class GetReportFundsExpendituresSettingsHandler
    : IRequestHandler<GetReportFundsExpendituresSettingsQuery, Result<ReportFundsExpendituresSettingsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetReportFundsExpendituresSettingsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<ReportFundsExpendituresSettingsDto>> Handle(
        GetReportFundsExpendituresSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settingsResult = await ReportFundsExpendituresSettingsHelper.GetOrCreateSettingsAsync(_repositoryWrapper);
        if (settingsResult.IsFailed)
        {
            return Result.Fail<ReportFundsExpendituresSettingsDto>(settingsResult.Errors);
        }

        return Result.Ok(_mapper.Map<ReportFundsExpendituresSettingsDto>(settingsResult.Value));
    }
}
