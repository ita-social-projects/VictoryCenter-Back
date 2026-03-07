using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

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
        var settings = await _repositoryWrapper.ReportFundsExpendituresSettingsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresSettingsEntity>
            {
                Filter = entity => entity.Id == ReportFundsExpendituresSettingsConstants.SingletonSettingsId
            });

        if (settings is null)
        {
            return Result.Fail<ReportFundsExpendituresSettingsDto>(
                ErrorMessagesConstants.NotFound(
                    ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
                    typeof(ReportFundsExpendituresSettingsEntity)));
        }

        return Result.Ok(_mapper.Map<ReportFundsExpendituresSettingsDto>(settings));
    }
}
