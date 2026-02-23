using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.ReportMediaSettings.GetAll;
public class GetReportMediaSettingsHandler
    : IRequestHandler<GetReportMediaSettingsQuery, Result<ReportMediaSettingsDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetReportMediaSettingsHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ReportMediaSettingsDto>> Handle(
        GetReportMediaSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var collectedFundsEntity = await _repositoryWrapper
            .GetRepository<CollectedFundsBlock>()
            .GetFirstOrDefaultAsync(new QueryOptions<CollectedFundsBlock>
            {
                Include = q => q.Include(b => b.Image!)
            });

        var changedLivesEntity = await _repositoryWrapper
            .GetRepository<ChangedLivesBlock>()
            .GetFirstOrDefaultAsync(new QueryOptions<ChangedLivesBlock>
            {
                Include = q => q.Include(b => b.Image!)
            });

        var resultDto = new ReportMediaSettingsDto
        {
            CollectedFundsBlock = _mapper.Map<CollectedFundsBlockDto>(collectedFundsEntity),
            ChangedLivesBlock = _mapper.Map<ChangedLivesBlockDto>(changedLivesEntity)
        };

        return Result.Ok(resultDto);
    }
}
