using Moq;
using VictoryCenter.BLL.Queries.Admin.ReportProgramExpendituresRecords.GetSummary;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class GetReportProgramExpendituresSummaryTests
{
    private readonly Mock<IReportProgramExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    public GetReportProgramExpendituresSummaryTests()
    {
        _recordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSummary()
    {
        // Arrange
        SetupSummary(125000.50m, 3012.25m);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetReportProgramExpendituresSummaryQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(125000.50m, result.Value.TotalAmountUah);
        Assert.Equal(3012.25m, result.Value.TotalAmountUsd);

        _recordsRepositoryMock.Verify(repository => repository.GetSummaryAsync(), Times.Once);
    }

    private GetReportProgramExpendituresSummaryHandler CreateHandler()
    {
        return new GetReportProgramExpendituresSummaryHandler(_repositoryWrapperMock.Object);
    }

    private void SetupSummary(decimal totalAmountUah, decimal totalAmountUsd)
    {
        _repositoryWrapperMock
            .SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _recordsRepositoryMock
            .Setup(repository => repository.GetSummaryAsync())
            .ReturnsAsync((totalAmountUah, totalAmountUsd));
    }
}
