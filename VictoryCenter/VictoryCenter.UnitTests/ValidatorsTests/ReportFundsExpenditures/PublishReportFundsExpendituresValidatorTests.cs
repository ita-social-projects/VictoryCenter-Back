using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Publish;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpenditures;

public class PublishReportFundsExpendituresValidatorTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _fundsRecordsRepositoryMock;
    private readonly Mock<IReportProgramExpendituresRecordsRepository> _programRecordsRepositoryMock;

    public PublishReportFundsExpendituresValidatorTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _fundsRecordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
        _programRecordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();

        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresRecordsRepository).Returns(_fundsRecordsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.ReportProgramExpendituresRecordsRepository).Returns(_programRecordsRepositoryMock.Object);
    }

    [Fact]
    public async Task Validate_ShouldNotHaveError_WhenAllConditionsMet()
    {
        // Arrange
        _fundsRecordsRepositoryMock.Setup(r => r.CountAsync(It.Is<QueryOptions<ReportFundsExpendituresRecord>>(q =>
            q.Filter != null && q.Filter.Compile().Invoke(new ReportFundsExpendituresRecord { Type = ReportFundsExpendituresType.Expense }))))
            .ReturnsAsync(2);

        _fundsRecordsRepositoryMock.Setup(r => r.CountAsync(It.Is<QueryOptions<ReportFundsExpendituresRecord>>(q =>
            q.Filter != null && q.Filter.Compile().Invoke(new ReportFundsExpendituresRecord { Type = ReportFundsExpendituresType.Income }))))
            .ReturnsAsync(2);

        _programRecordsRepositoryMock.Setup(r => r.CountAsync(null))
            .ReturnsAsync(1);

        var validator = new PublishReportFundsExpendituresValidator(_repositoryWrapperMock.Object);

        // Act
        var result = await validator.TestValidateAsync(new PublishReportFundsExpendituresCommand());

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenNotEnoughExpenseRecords()
    {
        // Arrange
        _fundsRecordsRepositoryMock.Setup(r => r.CountAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()))
            .ReturnsAsync(1);

        _programRecordsRepositoryMock.Setup(r => r.CountAsync(null))
            .ReturnsAsync(1);

        var validator = new PublishReportFundsExpendituresValidator(_repositoryWrapperMock.Object);

        // Act
        var result = await validator.TestValidateAsync(new PublishReportFundsExpendituresCommand());

        // Assert
        result.ShouldHaveValidationErrorFor(c => c)
            .WithErrorMessage("At least 2 expense records are required to publish.");
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenNotEnoughProgramRecords()
    {
        // Arrange
        _fundsRecordsRepositoryMock.Setup(r => r.CountAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()))
            .ReturnsAsync(2);

        _programRecordsRepositoryMock.Setup(r => r.CountAsync(null))
            .ReturnsAsync(0);

        var validator = new PublishReportFundsExpendituresValidator(_repositoryWrapperMock.Object);

        // Act
        var result = await validator.TestValidateAsync(new PublishReportFundsExpendituresCommand());

        // Assert
        result.ShouldHaveValidationErrorFor(c => c)
            .WithErrorMessage("At least 1 program expenditure record is required to publish.");
    }
}
