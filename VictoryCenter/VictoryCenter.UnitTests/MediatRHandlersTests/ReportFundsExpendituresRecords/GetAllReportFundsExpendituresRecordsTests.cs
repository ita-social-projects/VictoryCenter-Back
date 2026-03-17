using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresRecords.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresRecords;

public class GetAllReportFundsExpendituresRecordsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;

    private readonly List<ReportFundsExpendituresRecord> _records =
    [
        new()
        {
            Id = 1,
            CategoryId = 1,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 100,
            AmountUsd = 20
        },
        new()
        {
            Id = 2,
            CategoryId = 2,
            Type = ReportFundsExpendituresType.Expense,
            ReportingYear = 2025,
            AmountUah = 50,
            AmountUsd = 10
        }

    ];

    private readonly List<ReportFundsExpendituresRecordDto> _recordDtos =
    [
        new()
        {
            Id = 1,
            CategoryId = 1,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 100,
            AmountUsd = 20
        },
        new()
        {
            Id = 2,
            CategoryId = 2,
            Type = ReportFundsExpendituresType.Expense,
            ReportingYear = 2025,
            AmountUah = 50,
            AmountUsd = 10
        }

    ];

    public GetAllReportFundsExpendituresRecordsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllRecords()
    {
        // Arrange
        SetupDependencies(_records, _recordDtos);
        var handler = new GetAllReportFundsExpendituresRecordsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportFundsExpendituresRecordsQuery(),
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
        SetupDependencies([], []);
        var handler = new GetAllReportFundsExpendituresRecordsHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportFundsExpendituresRecordsQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupDependencies(
        IEnumerable<ReportFundsExpendituresRecord> records,
        IEnumerable<ReportFundsExpendituresRecordDto> recordDtos)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()))
            .ReturnsAsync(records);

        _mapperMock
            .Setup(mapper => mapper.Map<IEnumerable<ReportFundsExpendituresRecordDto>>(
                It.IsAny<IEnumerable<ReportFundsExpendituresRecord>>()))
            .Returns(recordDtos);
    }
}
