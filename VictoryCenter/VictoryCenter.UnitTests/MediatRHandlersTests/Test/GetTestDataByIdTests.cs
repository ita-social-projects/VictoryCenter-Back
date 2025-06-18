using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.BLL.Queries.Test.GetTestData;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Test;

public class GetTestDataByIdTests
{
    private const int TestId = 1;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly TestEntity _testEntity = new() { Id = TestId, TestName = "TestName" };
    private readonly TestDataDto _testDataDto = new() { TestName = "TestName" };

    public GetTestDataByIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_GetTestDataById_ReturnsTestData()
    {
        SetupDependencies(_testDataDto, _testEntity);
        var handler = new GetTestDataHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        var result = await handler.Handle(new GetTestDataQuery(TestId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(_testEntity.TestName == result.Value.TestName);
    }

    [Fact]
    public async Task Handle_GetTestDataById_ReturnsTestData_Null()
    {
        SetupDependencies(null, null);
        var handler = new GetTestDataHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        var result = await handler.Handle(new GetTestDataQuery(-1), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
    }

    private void SetupDependencies(TestDataDto? testDataDto, TestEntity? testEntity)
    {
        SetupMapper(testDataDto);
        SetupRepositoryWrapper(testEntity);
    }

    private void SetupMapper(TestDataDto? testDataDto)
    {
        _mockMapper.Setup(mapper => mapper.Map<TestDataDto>(It.IsAny<TestEntity>()))
            .Returns(testDataDto!);
    }

    private void SetupRepositoryWrapper(TestEntity? testEntity)
    {
        _mockRepositoryWrapper.Setup(repository => repository.TestRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<TestEntity>>()))
            .ReturnsAsync(testEntity);
    }
}
