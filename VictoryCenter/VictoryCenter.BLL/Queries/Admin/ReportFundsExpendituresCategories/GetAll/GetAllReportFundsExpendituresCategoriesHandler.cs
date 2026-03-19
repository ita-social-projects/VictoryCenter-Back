using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresCategories.GetAll;

public class GetAllReportFundsExpendituresCategoriesHandler
    : IRequestHandler<GetAllReportFundsExpendituresCategoriesQuery, Result<IEnumerable<ReportFundsExpendituresCategoryDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllReportFundsExpendituresCategoriesHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<ReportFundsExpendituresCategoryDto>>> Handle(
        GetAllReportFundsExpendituresCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository.GetAllAsync();

        return Result.Ok(_mapper.Map<IEnumerable<ReportFundsExpendituresCategoryDto>>(categories));
    }
}
