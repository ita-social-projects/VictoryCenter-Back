using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Test.CreateTestData;

public class CreateTestDataHandler : IRequestHandler<CreateTestDataCommand, Result<TestDataDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    
    public CreateTestDataHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }
    
    public async Task<Result<TestDataDto>> Handle(CreateTestDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var testEntity = _mapper.Map<TestEntity>(request.CreateTestData);
            var createdTestEntity = await _repositoryWrapper.TestRepository.CreateAsync(testEntity);
            await _repositoryWrapper.SaveChangesAsync();
            return Result.Ok(_mapper.Map<TestDataDto>(createdTestEntity));
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
