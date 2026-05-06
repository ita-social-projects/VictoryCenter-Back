using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.ReportFundsExpendituresCategories.GetByEntityId;

public class GetReportFundsExpendituresCategoryLocalizationByEntityIdHandler
    : IRequestHandler<GetReportFundsExpendituresCategoryLocalizationByEntityIdQuery, Result<List<ReportFundsExpendituresCategoryLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetReportFundsExpendituresCategoryLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<ReportFundsExpendituresCategoryLocalizationDto>>> Handle(
        GetReportFundsExpendituresCategoryLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<ReportFundsExpendituresCategoryLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = await _repositoryWrapper.ReportFundsExpendituresCategoryLocalizationsRepository.GetAllAsync(queryOptions);
        var responseDto = _mapper.Map<List<ReportFundsExpendituresCategoryLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
