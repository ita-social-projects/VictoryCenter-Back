using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Cancel;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.BackupReportFundsExpenditures;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpenditures;

public class CancelReportFundsExpendituresHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBackupReportFundsExpendituresSettingsRepository> _backupSettingsRepositoryMock;
    private readonly Mock<IBackupReportFundsExpendituresSettingsLocalizationsRepository> _backupSettingsLocalizationsRepositoryMock;
    private readonly Mock<IBackupReportFundsExpendituresRecordsRepository> _backupFundsRecordsRepositoryMock;
    private readonly Mock<IBackupReportProgramExpendituresRecordsRepository> _backupProgramRecordsRepositoryMock;

    private readonly Mock<IReportFundsExpendituresSettingsRepository> _settingsRepositoryMock;
    private readonly Mock<IReportFundsExpendituresSettingsLocalizationsRepository> _settingsLocalizationsRepositoryMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;
    private readonly Mock<IReportFundsExpendituresCategoryLocalizationsRepository> _categoryLocalizationsRepositoryMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _fundsRecordsRepositoryMock;
    private readonly Mock<IReportProgramExpendituresRecordsRepository> _programRecordsRepositoryMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;

    private readonly TimeProvider _timeProvider;

    public CancelReportFundsExpendituresHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _backupSettingsRepositoryMock = new Mock<IBackupReportFundsExpendituresSettingsRepository>();
        _backupSettingsLocalizationsRepositoryMock = new Mock<IBackupReportFundsExpendituresSettingsLocalizationsRepository>();
        _backupFundsRecordsRepositoryMock = new Mock<IBackupReportFundsExpendituresRecordsRepository>();
        _backupProgramRecordsRepositoryMock = new Mock<IBackupReportProgramExpendituresRecordsRepository>();

        _settingsRepositoryMock = new Mock<IReportFundsExpendituresSettingsRepository>();
        _settingsLocalizationsRepositoryMock = new Mock<IReportFundsExpendituresSettingsLocalizationsRepository>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
        _categoryLocalizationsRepositoryMock = new Mock<IReportFundsExpendituresCategoryLocalizationsRepository>();
        _fundsRecordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
        _programRecordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
        _transactionMock = new Mock<IDbContextTransaction>();

        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresSettingsRepository).Returns(_backupSettingsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresSettingsLocalizationsRepository).Returns(_backupSettingsLocalizationsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresRecordsRepository).Returns(_backupFundsRecordsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.BackupReportProgramExpendituresRecordsRepository).Returns(_backupProgramRecordsRepositoryMock.Object);

        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresSettingsRepository).Returns(_settingsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresSettingsLocalizationsRepository).Returns(_settingsLocalizationsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresCategoriesRepository).Returns(_categoriesRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresCategoryLocalizationsRepository).Returns(_categoryLocalizationsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresRecordsRepository).Returns(_fundsRecordsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(w => w.ReportProgramExpendituresRecordsRepository).Returns(_programRecordsRepositoryMock.Object);

        _repositoryWrapperMock.Setup(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);

        _settingsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings>>()))
            .ReturnsAsync(new global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings { Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId });

        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenBackupSettingsNotFound()
    {
        // Arrange
        _backupSettingsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<BackupReportFundsExpendituresSettings>>()))
            .ReturnsAsync((BackupReportFundsExpendituresSettings?)null);

        var handler = new CancelReportFundsExpendituresHandler(_repositoryWrapperMock.Object, _timeProvider);

        // Act
        var result = await handler.Handle(new CancelReportFundsExpendituresCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.CannotCancelChangesNoBackupFound(), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldCancelSuccessfully_WhenDataExists()
    {
        // Arrange
        var backupSettings = new BackupReportFundsExpendituresSettings { DisclaimerTitle = "Backup Title" };
        _backupSettingsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<BackupReportFundsExpendituresSettings>>()))
            .ReturnsAsync(backupSettings);

        var backupSettingsLocalizations = new List<BackupReportFundsExpendituresSettingsLocalization>
        {
            new() { LanguageId = 1, DisclaimerTitle = "Title EN" }
        };
        _backupSettingsLocalizationsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<BackupReportFundsExpendituresSettingsLocalization>>()))
            .ReturnsAsync(backupSettingsLocalizations);

        var backupFundsRecords = new List<BackupReportFundsExpendituresRecord>
        {
            new() { CategoryId = 1, AmountUah = 100 }
        };
        _backupFundsRecordsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<BackupReportFundsExpendituresRecord>>()))
            .ReturnsAsync(backupFundsRecords);

        var backupProgramRecords = new List<BackupReportProgramExpendituresRecord>
        {
            new() { HippotherapyProgramCategoryId = 1, AmountUah = 200 }
        };
        _backupProgramRecordsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<BackupReportProgramExpendituresRecord>>()))
            .ReturnsAsync(backupProgramRecords);

        var handler = new CancelReportFundsExpendituresHandler(_repositoryWrapperMock.Object, _timeProvider);

        // Act
        var result = await handler.Handle(new CancelReportFundsExpendituresCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _fundsRecordsRepositoryMock.Verify(r => r.BulkDeleteAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ReportFundsExpendituresRecord, bool>>>()), Times.Once);
        _programRecordsRepositoryMock.Verify(r => r.BulkDeleteAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ReportProgramExpendituresRecord, bool>>>()), Times.Once);
        _fundsRecordsRepositoryMock.Verify(r => r.CreateRangeAsync(It.IsAny<ReportFundsExpendituresRecord[]>()), Times.Once);
        _categoriesRepositoryMock.Verify(r => r.CreateRangeAsync(It.IsAny<ReportFundsExpendituresCategory[]>()), Times.Never);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.AtLeastOnce);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        _backupSettingsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<BackupReportFundsExpendituresSettings>>()))
            .ReturnsAsync(new BackupReportFundsExpendituresSettings());

        _repositoryWrapperMock.Setup(w => w.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new CancelReportFundsExpendituresHandler(_repositoryWrapperMock.Object, _timeProvider);

        // Act
        var result = await handler.Handle(new CancelReportFundsExpendituresCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Failed to cancel report funds expenditures changes.", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenExceptionOccurs()
    {
        // Arrange
        _backupSettingsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<BackupReportFundsExpendituresSettings>>()))
            .ReturnsAsync(new BackupReportFundsExpendituresSettings());

        _repositoryWrapperMock.Setup(w => w.SaveChangesAsync())
            .ThrowsAsync(new Exception());

        var handler = new CancelReportFundsExpendituresHandler(_repositoryWrapperMock.Object, _timeProvider);

        // Act
        var result = await handler.Handle(new CancelReportFundsExpendituresCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("An unexpected error occurred while canceling report funds expenditures changes.", result.Errors[0].Message);
    }
}
