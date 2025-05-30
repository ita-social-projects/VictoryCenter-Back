using AutoMapper;
using Moq;
using VictoryCenter.BLL.Commands.Test.CreateTestData;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Test;

public class CreateTestDataTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    
    private const string TestName = "TestName";
    private readonly TestEntity _testEntity = new () {TestName = TestName};
    private readonly TestDataDto _testDataDto = new () {TestName = TestName};
    
    public CreateTestDataTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_CreateTestData_ReturnsTestData()
    {
        SetupDependencies(_testEntity, _testDataDto);
        var handler = new CreateTestDataHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
        
        var result = await handler.Handle(new CreateTestDataCommand(new CreateTestDataDto() {TestName = TestName}), CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(_testEntity.TestName == result.Value.TestName);
    }

    [Fact]
    public async Task Handle_CreateTestData_ShouldFail()
    {
        SetupDependencies(_testEntity, _testDataDto, -1);
        var handler = new CreateTestDataHandler(_mapperMock.Object, _repositoryWrapperMock.Object);
        
        var result = await handler.Handle(new CreateTestDataCommand(new CreateTestDataDto() {TestName = TestName}), CancellationToken.None);
        
        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
    }
    
    private void SetupDependencies(TestEntity testEntity, TestDataDto testDataDto, int isSuccess = 1)
    {
        SetupMapper(testEntity, testDataDto);
        SetupRepositoryWrapper(testEntity, isSuccess);
    }

    private void SetupMapper(TestEntity testEntity, TestDataDto testDataDto)
    {
        _mapperMock.Setup(mapper => mapper.Map<TestEntity>(It.IsAny<CreateTestDataDto>())).Returns(testEntity);
        _mapperMock.Setup(mapper => mapper.Map<TestDataDto>(It.IsAny<TestEntity>())).Returns(testDataDto);
    }

    private void SetupRepositoryWrapper(TestEntity testEntity, int isSuccess)
    {
        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.TestRepository
            .CreateAsync(It.IsAny<TestEntity>()))
            .ReturnsAsync(testEntity);
        
        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.SaveChangesAsync())
            .ReturnsAsync(isSuccess);
    }
}