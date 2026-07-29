using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpenditures.Publish;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.BackupReportFundsExpenditures;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportFundsExpendituresSnapshot;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpenditures;

public class PublishReportFundsExpendituresHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<PublishReportFundsExpendituresCommand>> _validatorMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly TimeProvider _timeProvider;

    public PublishReportFundsExpendituresHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validatorMock = new Mock<IValidator<PublishReportFundsExpendituresCommand>>();
        _transactionMock = new Mock<IDbContextTransaction>();
        _timeProvider = TimeProvider.System;

        _repositoryWrapperMock.Setup(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPublishSuccessfully_WhenDataIsValid()
    {
        // Arrange
        var fundsRecords = new List<ReportFundsExpendituresRecord>
        {
            new()
            {
                Id = 1,
                Category = new ReportFundsExpendituresCategory
                {
                    Name = "Cat",
                    Localizations = new List<ReportFundsExpendituresCategoryLocalization>
                    {
                        new() { Language = new LocalizationLanguage { Code = "en" }, Name = "Cat EN" }
                    }
                }
            }
        };

        var programRecords = new List<ReportProgramExpendituresRecord>
        {
            new()
            {
                Id = 1,
                HippotherapyProgramCategory = new HippotherapyProgramCategory { Name = "ProgCat" }
            }
        };

        var settings = new global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings
        {
            Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
            DisclaimerTitle = "Disclaimer"
        };

        var settingsLocalizations = new List<ReportFundsExpendituresSettingsLocalization>
        {
            new() { Language = new LocalizationLanguage { Code = "en" }, DisclaimerTitle = "Disclaimer EN" }
        };

        var categories = new List<ReportFundsExpendituresCategory>();

        SetupRepository<IReportFundsExpendituresRecordsRepository, ReportFundsExpendituresRecord>(
            w => w.ReportFundsExpendituresRecordsRepository, fundsRecords);

        SetupRepository<IReportProgramExpendituresRecordsRepository, ReportProgramExpendituresRecord>(
            w => w.ReportProgramExpendituresRecordsRepository, programRecords);

        SetupRepository<IReportFundsExpendituresSettingsLocalizationsRepository, ReportFundsExpendituresSettingsLocalization>(
            w => w.ReportFundsExpendituresSettingsLocalizationsRepository, settingsLocalizations);

        SetupRepository<IReportFundsExpendituresCategoriesRepository, ReportFundsExpendituresCategory>(
            w => w.ReportFundsExpendituresCategoriesRepository, categories);

        var settingsRepoMock = new Mock<IReportFundsExpendituresSettingsRepository>();
        settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<global::VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings>>()))
            .ReturnsAsync(settings);
        _repositoryWrapperMock.Setup(w => w.ReportFundsExpendituresSettingsRepository).Returns(settingsRepoMock.Object);

        var publishedFundsRepoMock = new Mock<IPublishedReportFundsExpendituresRecordsRepository>();
        _repositoryWrapperMock.Setup(w => w.PublishedReportFundsExpendituresRecordsRepository).Returns(publishedFundsRepoMock.Object);
        var publishedProgramRepoMock = new Mock<IPublishedReportProgramExpendituresRecordsRepository>();
        _repositoryWrapperMock.Setup(w => w.PublishedReportProgramExpendituresRecordsRepository).Returns(publishedProgramRepoMock.Object);
        var publishedSnapshotRepoMock = new Mock<IPublishedReportFundsExpendituresSnapshotRepository>();
        _repositoryWrapperMock.Setup(w => w.PublishedReportFundsExpendituresSnapshotRepository).Returns(publishedSnapshotRepoMock.Object);

        var backupFundsRepoMock = new Mock<IBackupReportFundsExpendituresRecordsRepository>();
        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresRecordsRepository).Returns(backupFundsRepoMock.Object);
        var backupProgramRepoMock = new Mock<IBackupReportProgramExpendituresRecordsRepository>();
        _repositoryWrapperMock.Setup(w => w.BackupReportProgramExpendituresRecordsRepository).Returns(backupProgramRepoMock.Object);
        var backupCatLocsRepoMock = new Mock<IBackupReportFundsExpendituresCategoryLocalizationsRepository>();
        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresCategoryLocalizationsRepository).Returns(backupCatLocsRepoMock.Object);
        var backupCatsRepoMock = new Mock<IBackupReportFundsExpendituresCategoriesRepository>();
        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresCategoriesRepository).Returns(backupCatsRepoMock.Object);
        var backupSetLocsRepoMock = new Mock<IBackupReportFundsExpendituresSettingsLocalizationsRepository>();
        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresSettingsLocalizationsRepository).Returns(backupSetLocsRepoMock.Object);
        var backupSettingsRepoMock = new Mock<IBackupReportFundsExpendituresSettingsRepository>();
        _repositoryWrapperMock.Setup(w => w.BackupReportFundsExpendituresSettingsRepository).Returns(backupSettingsRepoMock.Object);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var handler = new PublishReportFundsExpendituresHandler(
            _repositoryWrapperMock.Object,
            _validatorMock.Object,
            _timeProvider);

        // Act
        var result = await handler.Handle(new PublishReportFundsExpendituresCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        publishedFundsRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<PublishedReportFundsExpendituresRecord[]>()), Times.Once);
        publishedProgramRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<PublishedReportProgramExpendituresRecord[]>()), Times.Once);
        publishedSnapshotRepoMock.Verify(r => r.CreateAsync(It.IsAny<PublishedReportFundsExpendituresSnapshot>()), Times.Once);

        backupFundsRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<BackupReportFundsExpendituresRecord[]>()), Times.Once);
        backupProgramRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<BackupReportProgramExpendituresRecord[]>()), Times.Once);
        backupCatsRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<BackupReportFundsExpendituresCategory[]>()), Times.Once);
        backupCatLocsRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<BackupReportFundsExpendituresCategoryLocalization[]>()), Times.Once);
        backupSettingsRepoMock.Verify(r => r.CreateAsync(It.IsAny<BackupReportFundsExpendituresSettings>()), Times.Once);
        backupSetLocsRepoMock.Verify(r => r.CreateRangeAsync(It.IsAny<BackupReportFundsExpendituresSettingsLocalization[]>()), Times.Once);

        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.AtLeastOnce);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenValidationFails()
    {
        // Arrange
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[] { new FluentValidation.Results.ValidationFailure("Prop", "Error") }));

        var handler = new PublishReportFundsExpendituresHandler(_repositoryWrapperMock.Object, _validatorMock.Object, _timeProvider);

        // Act
        var result = await handler.Handle(new PublishReportFundsExpendituresCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("An unexpected error occurred while publishing report funds expenditures data.", result.Errors[0].Message);
    }

    private void SetupRepository<TRepo, TEntity>(System.Linq.Expressions.Expression<Func<IRepositoryWrapper, TRepo>> expression, List<TEntity> items)
        where TRepo : class, IRepositoryBase<TEntity>
        where TEntity : class
    {
        var mock = new Mock<TRepo>();
        mock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<TEntity>>())).ReturnsAsync(items);
        _repositoryWrapperMock.Setup(expression).Returns(mock.Object);
    }
}
