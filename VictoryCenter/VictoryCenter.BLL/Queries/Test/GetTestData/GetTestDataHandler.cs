using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Test.GetTestData;

public class GetTestDataHandler : IRequestHandler<GetTestDataQuery, Result<TestDataDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetTestDataHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<TestDataDto>> Handle(GetTestDataQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var testEntity =
                await _repositoryWrapper.TestRepository.GetFirstOrDefaultAsync(new QueryOptions<TestEntity>
                {
                    FilterPredicate = entity => entity.Id == request.Id
                });

            if (testEntity is null)
            {
                return Result.Fail("Test not found");
            }

            return Result.Ok(_mapper.Map<TestDataDto>(testEntity));
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
