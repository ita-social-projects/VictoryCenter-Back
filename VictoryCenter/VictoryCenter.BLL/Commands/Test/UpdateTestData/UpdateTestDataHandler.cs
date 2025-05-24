using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

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
            var existingEntity = await _repositoryWrapper.TestRepository.GetFirstOrDefaultAsync(entity => entity.Id == request.UpdateTestData.Id);
            if (existingEntity is null)
            {
                return Result.Fail("Entity not found");
            }
            
            var testEntity = _mapper.Map<TestEntity>(request.UpdateTestData);
            var updatedTestEntity = _repositoryWrapper.TestRepository.Update(testEntity);
            await _repositoryWrapper.SaveChangesAsync();
            return Result.Ok(_mapper.Map<TestDataDto>(updatedTestEntity.Entity));
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
