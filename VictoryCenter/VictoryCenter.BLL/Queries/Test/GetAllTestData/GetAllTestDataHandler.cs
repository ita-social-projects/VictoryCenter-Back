using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Test.GetAllTestData;

public class GetAllTestDataHandler : IRequestHandler<GetAllTestDataQuery, Result<IEnumerable<TestDataDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllTestDataHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<IEnumerable<TestDataDto>>> Handle(GetAllTestDataQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var allTestEntities = await _repositoryWrapper.TestRepository.GetAllAsync();
            return Result.Ok(_mapper.Map<IEnumerable<TestDataDto>>(allTestEntities));
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
