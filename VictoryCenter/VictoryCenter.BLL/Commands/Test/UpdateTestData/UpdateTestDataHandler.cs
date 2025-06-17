using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Test.UpdateTestData;

public class UpdateTestDataHandler : IRequestHandler<UpdateTestDataCommand, Result<TestDataDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateTestDataHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<TestDataDto>> Handle(UpdateTestDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingEntity = await _repositoryWrapper.TestRepository.GetFirstOrDefaultAsync(new QueryOptions<TestEntity>
            {
                Filter = entity => entity.Id == request.UpdateTestData.Id
            });

            if (existingEntity is null)
            {
                return Result.Fail("Entity not found");
            }

            var testEntity = _mapper.Map<TestEntity>(request.UpdateTestData);
            _repositoryWrapper.TestRepository.Update(testEntity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var testDataDto = _mapper.Map<TestDataDto>(testEntity);
                return Result.Ok(testDataDto);
            }

            return Result.Fail("Failed to update test data");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
