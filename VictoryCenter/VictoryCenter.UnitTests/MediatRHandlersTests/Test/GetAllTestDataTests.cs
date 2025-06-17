using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.BLL.Queries.Test.GetAllTestData;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Test;

public class GetAllTestDataTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<TestEntity> _testEntities = [new TestEntity()];
    private readonly IEnumerable<TestDataDto> _testDataDtos = [new TestDataDto() { TestName = "TestName" }];

    public GetAllTestDataTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_GetAllTestData_ReturnsTestData_NotEmpty()
    {
        SetupDependencies(_testEntities, _testDataDtos);
        var handler = new GetAllTestDataHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        var result = await handler.Handle(new GetAllTestDataQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public async Task Handle_GetAllTestData_ReturnsTestData_Empty()
    {
        SetupDependencies([], []);
        var handler = new GetAllTestDataHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        var result = await handler.Handle(new GetAllTestDataQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    private void SetupDependencies(IEnumerable<TestEntity> testEntities, IEnumerable<TestDataDto> testDataDtos)
    {
        SetupMapper(testDataDtos);
        SetupRepositoryWrapper(testEntities);
    }

    private void SetupMapper(IEnumerable<TestDataDto> testDataDtos)
    {
        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TestDataDto>>(It.IsAny<IEnumerable<TestEntity>>()))
            .Returns(testDataDtos);
    }

    private void SetupRepositoryWrapper(IEnumerable<TestEntity> testEntities)
    {
        _mockRepositoryWrapper.Setup(repositoryWrapper => repositoryWrapper.TestRepository.GetAllAsync(
             It.IsAny<Func<IQueryable<TestEntity>, IIncludableQueryable<TestEntity, object>>>(),
             It.IsAny<Expression<Func<TestEntity, bool>>>(),
             It.IsAny<int>(),
             It.IsAny<int>(),
             It.IsAny<Expression<Func<TestEntity, object>>>(),
             It.IsAny<Expression<Func<TestEntity, object>>>(),
             It.IsAny<Expression<Func<TestEntity, TestEntity>>>()))
            .ReturnsAsync(testEntities);
    }
}
