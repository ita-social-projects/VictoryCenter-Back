using System.Linq.Expressions;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Queries.Admin.ReportProgramExpendituresRecords.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class GetAllReportProgramExpendituresRecordsTests
{
    private readonly List<ReportProgramExpendituresRecordDto> _recordDtos =
    [
        new()
        {
            Id = 1,
            HippotherapyProgramCategoryId = 1,
            ReportingYear = 2024,
            AmountUah = 100.50m,
            AmountUsd = 25.25m
        },
        new()
        {
            Id = 2,
            HippotherapyProgramCategoryId = 2,
            ReportingYear = 2024,
            AmountUah = 200.00m,
            AmountUsd = 50.00m
        }

    ];

    private readonly Mock<IReportProgramExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetAllReportProgramExpendituresRecordsTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllRecords()
    {
        // Arrange
        SetupDependencies(_recordDtos);
        var handler = new GetAllReportProgramExpendituresRecordsHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportProgramExpendituresRecordsQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_recordDtos.Count, result.Value.Count());
        Assert.Equal(_recordDtos[0].Id, result.Value.ElementAt(0).Id);
        Assert.Equal(_recordDtos[1].Id, result.Value.ElementAt(1).Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoRecordsExist()
    {
        // Arrange
        SetupDependencies([]);
        var handler = new GetAllReportProgramExpendituresRecordsHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportProgramExpendituresRecordsQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFilterByCategory_WhenCategoryIdProvided()
    {
        // Arrange
        QueryOptions<ReportProgramExpendituresRecord>? capturedOptions = null;

        _repositoryWrapperMock.SetupGet(w => w.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(r => r.GetAllProjectedAsync(
                It.IsAny<Expression<Func<ReportProgramExpendituresRecord, ReportProgramExpendituresRecordDto>>>(),
                It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .Callback<Expression<Func<ReportProgramExpendituresRecord, ReportProgramExpendituresRecordDto>>,
                QueryOptions<ReportProgramExpendituresRecord>>((_, opts) => capturedOptions = opts)
            .ReturnsAsync(_recordDtos.Where(r => r.HippotherapyProgramCategoryId == 1).ToList());

        var handler = new GetAllReportProgramExpendituresRecordsHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportProgramExpendituresRecordsQuery(1),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);

        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions.Filter);
        Assert.Equal(ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerOneRetrieval, capturedOptions.Limit);
        Assert.True(capturedOptions.AsNoTracking);

        var predicate = capturedOptions.Filter!.Compile();
        Assert.True(predicate(new ReportProgramExpendituresRecord { HippotherapyProgramCategoryId = 1 }));
        Assert.False(predicate(new ReportProgramExpendituresRecord { HippotherapyProgramCategoryId = 2 }));
    }

    [Fact]
    public async Task Handle_ShouldReturnAllRecords_WhenCategoryIdIsNull()
    {
        // Arrange
        QueryOptions<ReportProgramExpendituresRecord>? capturedOptions = null;

        _repositoryWrapperMock.SetupGet(w => w.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(r => r.GetAllProjectedAsync(
                It.IsAny<Expression<Func<ReportProgramExpendituresRecord, ReportProgramExpendituresRecordDto>>>(),
                It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .Callback<Expression<Func<ReportProgramExpendituresRecord, ReportProgramExpendituresRecordDto>>,
                QueryOptions<ReportProgramExpendituresRecord>>((_, opts) => capturedOptions = opts)
            .ReturnsAsync(_recordDtos);

        var handler = new GetAllReportProgramExpendituresRecordsHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportProgramExpendituresRecordsQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_recordDtos.Count, result.Value.Count());

        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions.Filter);
        Assert.Equal(ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerOneRetrieval, capturedOptions.Limit);
        Assert.True(capturedOptions.AsNoTracking);

        var predicate = capturedOptions.Filter!.Compile();
        Assert.True(predicate(new ReportProgramExpendituresRecord { HippotherapyProgramCategoryId = 1 }));
        Assert.True(predicate(new ReportProgramExpendituresRecord { HippotherapyProgramCategoryId = 99 }));
    }

    private void SetupDependencies(IEnumerable<ReportProgramExpendituresRecordDto> recordDtos)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(repository => repository.GetAllProjectedAsync(
                It.IsAny<Expression<Func<ReportProgramExpendituresRecord, ReportProgramExpendituresRecordDto>>>(),
                It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .ReturnsAsync(recordDtos);
    }
}
