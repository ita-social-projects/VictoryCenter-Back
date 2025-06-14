using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using VictoryCenter.BLL.Commands.Test.UpdateTestData;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Test;

public class UpdateTestDataTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    
    private const int TestId = 1;
    
    private readonly UpdateTestDataDto _updateTestDataDto = new () {Id = TestId, TestName = "UpdatedTestName"};
    private readonly UpdateTestDataDto _invalidUpdateTestDataDto = new () {Id = -1, TestName = "UpdatedTestName"};
    private readonly TestEntity _testEntity = new () {Id = TestId, TestName = "TestName"};
    private readonly TestDataDto _testDataDto = new () {TestName = "UpdatedTestName"};
    
    public UpdateTestDataTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepository = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_UpdateTestData_ReturnsTestData()
    {
        SetupDependencies(_testEntity, _testDataDto);
        var handler = new UpdateTestDataHandler(_mockMapper.Object, _mockRepository.Object);
        
        var result = await handler.Handle(new UpdateTestDataCommand(_updateTestDataDto), CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(_testDataDto.TestName == result.Value.TestName);
    }

    [Fact]
    public async Task Handle_UpdateTestData_ReturnsTestData_EntityNotFound()
    {
        SetupDependencies(null, null);
        var handler = new UpdateTestDataHandler(_mockMapper.Object, _mockRepository.Object);
        
        var result = await handler.Handle(new UpdateTestDataCommand(_invalidUpdateTestDataDto), CancellationToken.None);
        
        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
    }

    [Fact]
    public async Task Handle_UpdateTestData_ShouldFail()
    {
        SetupDependencies(_testEntity, _testDataDto, -1);
        var handler = new UpdateTestDataHandler(_mockMapper.Object, _mockRepository.Object);
        
        var result = await handler.Handle(new UpdateTestDataCommand(_updateTestDataDto), CancellationToken.None);
        
        Assert.True(result.IsFailed);
    }
    
    private void SetupDependencies(TestEntity testEntity, TestDataDto testDataDto, int isSuccess = 1)
    {
        SetupMapper(testEntity, testDataDto);
        SetupRepositoryWrapper(testEntity, isSuccess);
    }
    
    private void SetupMapper(TestEntity testEntity, TestDataDto testDataDto)
    {
        _mockMapper.Setup(mapper => mapper.Map<TestEntity>(It.IsAny<UpdateTestDataDto>())).Returns(testEntity);
        _mockMapper.Setup(mapper => mapper.Map<TestDataDto>(It.IsAny<TestEntity?>())).Returns(testDataDto);
    }

    private void SetupRepositoryWrapper(TestEntity testEntity, int isSuccess)
    {
        _mockRepository.Setup(repository => repository.TestRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TestEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TestEntity>, IIncludableQueryable<TestEntity, object>>>()))
            .ReturnsAsync(testEntity);

        _mockRepository.Setup(repository => repository.TestRepository
            .Update(It.IsAny<TestEntity>()));

        _mockRepository.Setup(repository => repository.SaveChangesAsync())
            .ReturnsAsync(isSuccess);
    }
}